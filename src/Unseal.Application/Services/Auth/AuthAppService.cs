using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using OpenIddict.Abstractions;
using Unseal.Constants;
using Unseal.Dtos.Auth;
using Unseal.Enums;
using Unseal.Etos;
using Unseal.Interfaces.Managers.Auth;
using Unseal.Localization;
using Unseal.Profiles.Auth;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace Unseal.Services.Auth;

public class AuthAppService : UnsealAppService, IAuthAppService
{
    private IdentityUserManager IdentityUserManager =>
        LazyServiceProvider.LazyGetRequiredService<IdentityUserManager>();

    private ICustomIdentityUserManager CustomIdentityUserManager =>
        LazyServiceProvider.LazyGetRequiredService<ICustomIdentityUserManager>();

    private AuthMapper AuthMapper =>
        LazyServiceProvider.LazyGetRequiredService<AuthMapper>();

    private IConfiguration Configuration =>
        LazyServiceProvider.LazyGetRequiredService<IConfiguration>();

    private IDistributedEventBus DistributedEventBus =>
        LazyServiceProvider.LazyGetRequiredService<IDistributedEventBus>();

    private IStringLocalizer<UnsealResource> StringLocalizer =>
        LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<UnsealResource>>();

    private readonly IDataFilter<IMultiTenant> _dataFilter;

    public AuthAppService(
        IDataFilter<IMultiTenant> dataFilter
    )
    {
        _dataFilter = dataFilter;
    }

    public async Task<bool> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default
    )
    {
        using (_dataFilter.Disable())
        {
            await CheckUsernameInUseAsync(dto.Username);
            await CheckMailInUseAsync(dto.Email);
            var model = AuthMapper.MapToDto(dto);
            var user = await CustomIdentityUserManager.Create(
                GuidGenerator.Create(),
                Guid.Parse(TenantConstants.TenantId),
                model
            );
            var result = await IdentityUserManager.CreateAsync(user, model.Password);
            result.CheckErrors();
            using (_dataFilter.Disable())
            {
                await IdentityUserManager.AddDefaultRolesAsync(user);
            }

            var mailConfirmationToken = await IdentityUserManager
                .GenerateEmailConfirmationTokenAsync(user);
            var encodedToken =
                WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(mailConfirmationToken));
            var userRegisterEto = new UserRegisterEto
            {
                UserId = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                ConfirmationToken = encodedToken
            };
            var userElasticEto = new UserElasticEto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Surname = user.Surname,
                ProfilePictureUrl = null,
                BlockedUserId = null,
                UserElasticQueryTpe = (int)UserElasticQueryTypes.Create
            };
            var userProfileEto = new UserProfileEto()
            {
                UserId = user.Id,
            };
            await DistributedEventBus.PublishAsync(userRegisterEto);
            await DistributedEventBus.PublishAsync(userElasticEto);
            await DistributedEventBus.PublishAsync(userProfileEto);

            return result.Succeeded;
        }
    }

    private async Task CheckUsernameInUseAsync(string username)
    {
        var existingUser = await IdentityUserManager.FindByNameAsync(username);
        if (existingUser is not null)
        {
            throw new UserFriendlyException(StringLocalizer[ExceptionCodes.IdentityUser.UsernameInUsed]);
        }
    }
    private async Task CheckMailInUseAsync(string email)
    {
        var existingUser = await IdentityUserManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            throw new UserFriendlyException(StringLocalizer[ExceptionCodes.IdentityUser.MailInUsed]);
        }
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginDto loginDto,
        CancellationToken cancellationToken = default
    )
    {
        using (_dataFilter.Disable())
        {
            var user = await CustomIdentityUserManager.TryGetByAsync(x =>
                    string.Equals(x.Email, loginDto.Email),
                true,
                cancellationToken: cancellationToken);

            if (!user.IsActive)
            {
                return new LoginResponseDto
                {
                    IsActive = false
                };
            }
            
            if (!await IdentityUserManager.IsEmailConfirmedAsync(user))
            {
                throw new UserFriendlyException(
                    StringLocalizer[ExceptionCodes.IdentityUser.CannotLoginIfMailNotConfirmed]);
            }

            var tokenResponse = await GetTokenAsync(
                user.UserName,
                loginDto.Password,
                user.TenantId?.ToString(),
                cancellationToken
            );
            var response = new LoginResponseDto();
            if (!string.IsNullOrEmpty(tokenResponse.Raw))
            {
                using var doc = JsonDocument.Parse(tokenResponse.Raw);

                if (doc.RootElement.TryGetProperty("data", out var data))
                {
                    var accessToken = data.GetProperty(OpenIddictConstants.Parameters.AccessToken).GetString();
                    var refreshToken = data.TryGetProperty(OpenIddictConstants.Parameters.RefreshToken, out var rt)
                        ? rt.GetString()
                        : null;
                    var expiresIn = data.GetProperty(OpenIddictConstants.Parameters.ExpiresIn).GetInt32();

                    response = new LoginResponseDto
                    {
                        UserId = user.Id,
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        ExpireIn = expiresIn
                    };
                }
            }

            return response;
        }
    }

    public async Task<bool> ConfirmMailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        using (_dataFilter.Disable())
        {
            var user = await CustomIdentityUserManager.TryGetByAsync(x =>
                    x.Id.Equals(userId),
                true,
                cancellationToken: cancellationToken
            );

            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(token));
            var result =
                await IdentityUserManager.ConfirmEmailAsync(user, decodedToken);

            return result.Succeeded;
        }
    }

    public async Task<bool> UserDeleteAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var user = await CustomIdentityUserManager.TryGetByAsync(x => x.Id.Equals(userId), true,
            cancellationToken: cancellationToken);

        var roles = await IdentityUserManager.GetRolesAsync(user);
        if (!roles.IsNullOrEmpty() && roles.Any())
        {
            await IdentityUserManager.RemoveFromRolesAsync(user, roles);
        }

        var result = await IdentityUserManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            var userDeleteEto = new UserDeleteEto
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email
            };
            await DistributedEventBus.PublishAsync(userDeleteEto);
        }

        return result.Succeeded;
    }

    public async Task<bool> SendConfirmationMailAsync(
        string mail,
        CancellationToken cancellationToken = default)
    {
        using (_dataFilter.Disable())
        {
            var user = await CustomIdentityUserManager.TryGetByAsync(x =>
                    x.Email == mail, true,
                cancellationToken: cancellationToken);
            var token = await IdentityUserManager
                .GenerateEmailConfirmationTokenAsync(user);

            var encodedToken =
                WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var userRegisterEto = new UserRegisterEto
            {
                UserId = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                ConfirmationToken = encodedToken
            };
            await DistributedEventBus.PublishAsync(userRegisterEto);
            return true;
        }
    }

    public async Task<bool> ChangeMailAsync(
        Guid userId,
        string newMailAddress,
        CancellationToken cancellationToken = default
    )
    {
        await CheckMailInUseAsync(newMailAddress);
        var user = await CustomIdentityUserManager.TryGetByAsync(x =>
                x.Id.Equals(userId), true,
            cancellationToken: cancellationToken);
        var token = await IdentityUserManager.GenerateChangeEmailTokenAsync(user, newMailAddress);
        await DistributedEventBus.PublishAsync(new ConfirmChangeMailEto
        {
            UserId = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            NewMailAddress = newMailAddress,
            Token = token
        });
        await IdentityUserManager.ChangeEmailAsync(user, newMailAddress, token);
        return true;
    }

    public async Task<bool> ConfirmChangeMailAsync(
        Guid userId,
        string newMailAddress,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        using (_dataFilter.Disable())
        {
            var user = await CustomIdentityUserManager.TryGetByAsync(
                x => x.Id == userId,
                true,
                cancellationToken: cancellationToken
            );
            var decodedToken = WebUtility.UrlDecode(token);
            var result = await IdentityUserManager
                .ChangeEmailAsync(user, newMailAddress, decodedToken);

            return result.Succeeded;
        }
    }

    public async Task<bool> ChangePasswordAsync(
        ChangePasswordDto changePasswordDto,
        CancellationToken cancellationToken = default
    )
    {
        var userId = CurrentUser.GetId();
        var user = await IdentityUserManager.GetByIdAsync(userId);
        (await IdentityUserManager
            .ChangePasswordAsync(
                user,
                changePasswordDto.OldPassword,
                changePasswordDto.NewPassword
            )).CheckErrors();
        return true;
    }

    public async Task<bool> SendPasswordResetCodeAsync(
        string email,
        CancellationToken cancellationToken = default

    )
    {
        using (_dataFilter.Disable())
        {
            var user = await CustomIdentityUserManager.TryGetByAsync(
                x => x.Email == email,
                true,
                cancellationToken: cancellationToken
            );
            
            var resetToken = await IdentityUserManager.GeneratePasswordResetTokenAsync(user);
            
            var encodedToken =
                WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
            var passwordResetEto = new PasswordResetEto
            {
                UserId = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                ConfirmationToken = encodedToken
            };
            await DistributedEventBus.PublishAsync(passwordResetEto);
            return true;
        }
    }

    public async Task<bool> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler);

        var discovery = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = Configuration[AuthConstants.Authority],
            Policy = { RequireHttps = false }
        }, cancellationToken: cancellationToken);

        if (discovery.IsError) throw new UserFriendlyException(discovery.Exception.InnerException.Message);

        var response = await client.RevokeTokenAsync(new TokenRevocationRequest
        {
            Address = discovery.RevocationEndpoint,
            ClientId = Configuration[AuthConstants.SwaggerClientId]!,
            Token = refreshToken,
            TokenTypeHint = OpenIddictConstants.Parameters.RefreshToken
        }, cancellationToken: cancellationToken);

        return !response.IsError;
    }

    public async Task<bool> DeactivateAccountAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        var user = await CustomIdentityUserManager.TryGetByAsync(x =>
                x.Id.Equals(CurrentUser.GetId()),
            true,
            cancellationToken: cancellationToken);
        
        user.SetIsActive(false);
        var result = await IdentityUserManager.UpdateAsync(user);
        result.CheckErrors();
        await LogoutAsync(refreshToken, cancellationToken);
        return result.Succeeded;
    }

    public async Task<bool> SendActivityMailAsync(
        string email, 
        CancellationToken cancellationToken = default
    )
    {
        var user = await CustomIdentityUserManager.TryGetByAsync(x =>
                x.Id.Equals(CurrentUser.GetId()),
            true,
            cancellationToken: cancellationToken);
        
        var userActivationEto = new UserActivationEto()
        {
            UserId = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email
        };
        await DistributedEventBus.PublishAsync(userActivationEto);
        return true;
    }

    public async Task<bool> ConfirmActivationMailAsync(
        Guid userId, 
        CancellationToken cancellationToken = default
    )
    {
        using (_dataFilter.Disable())
        {
            var user = await CustomIdentityUserManager.TryGetByAsync(x =>
                    x.Id.Equals(userId),
                true,
                cancellationToken: cancellationToken);
            user.SetIsActive(true);
            var result = await IdentityUserManager.UpdateAsync(user);
            result.CheckErrors();
            return result.Succeeded;
        }
    }

    public async Task<bool> ConfirmPasswordResetAsync(
        Guid userId,
        string newPassword, 
        string token,
        CancellationToken cancellationToken = default
    )
    {
        using (_dataFilter.Disable())
        {
            var user = await CustomIdentityUserManager.TryGetByAsync(x =>
                    x.Id.Equals(userId),
                true,
                cancellationToken: cancellationToken);
            var decodedToken = WebUtility.UrlDecode(token);
            var result = await IdentityUserManager.ResetPasswordAsync(user, decodedToken, newPassword);
            result.CheckErrors();
            return result.Succeeded;
        }
    }

    private async Task<TokenResponse> GetTokenAsync(
        string username,
        string password,
        string? tenantId,
        CancellationToken cancellationToken = default
    )
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        var client = new HttpClient(handler);
        var discovery = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = Configuration[AuthConstants.Authority],
            Policy = { RequireHttps = false }
        }, cancellationToken);
        var request = new PasswordTokenRequest
        {
            Address = discovery.TokenEndpoint,
            ClientId = Configuration[AuthConstants.SwaggerClientId]!,
            UserName = username,
            Password = password,
            Scope = $"openid offline_access {AuthConstants.Scope}"
        };
        SetRequestHeaders(request, tenantId);
        var tokenResponse = await client
            .RequestPasswordTokenAsync(
                request,
                cancellationToken
            );

        if (tokenResponse.IsError)
        {
            throw new UserFriendlyException(tokenResponse.Error!);
        }

        return tokenResponse;
    }

    private void SetRequestHeaders(PasswordTokenRequest request, string? tenantId)
    {
        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            request.Address = request.Address + "?__tenant=" + tenantId;
        }
    }
}