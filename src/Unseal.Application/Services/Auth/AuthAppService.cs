using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Dtos.Auth;
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
        await CheckMailFormatAsync(dto.Email);
        await CheckMailInUseAsync(dto.Email);
        var model = AuthMapper.MapToDto(dto);
        var user = await CustomIdentityUserManager.Create(
            GuidGenerator.Create(),
            Guid.Parse(TenantConstants.TenantId),
            model
        );
        var result = await IdentityUserManager.CreateAsync(user, model.Password);
        result.CheckErrors();
        await IdentityUserManager.AddDefaultRolesAsync(user);

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
        await DistributedEventBus.PublishAsync(userRegisterEto);

        var userProfile = new UserProfileEto()
        {
            UserId = user.Id,
        };
        await DistributedEventBus.PublishAsync(userProfile);
        
        return result.Succeeded;
    }

    private async Task CheckMailFormatAsync(string email)
    { 
        var emailRegex = new Regex(
        RegexConstants.MailRegexFormat,
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (!emailRegex.IsMatch(email))
        {
            throw new UserFriendlyException(StringLocalizer[ExceptionCodes.IdentityUser.MailIsInvalid]);
        }
    }
    private async Task CheckMailInUseAsync(string email)
    {
        var existingUser = await IdentityUserManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            throw new UserFriendlyException(StringLocalizer[ExceptionCodes.IdentityUser.MailInUser]);
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
                    string.Equals(x.UserName, loginDto.UserName),
                true,
                cancellationToken: cancellationToken);
            if (!await IdentityUserManager.IsEmailConfirmedAsync(user))
            {
                throw new UserFriendlyException(
                    StringLocalizer[ExceptionCodes.IdentityUser.CannotLoginIfMailNotConfirmed]);
            }

            var tokenResponse = await GetTokenAsync(
                loginDto.UserName,
                loginDto.Password,
                user.TenantId?.ToString(),
                cancellationToken
            );

            var response = new LoginResponseDto(
                user.Id,
                tokenResponse.AccessToken,
                tokenResponse.RefreshToken,
                tokenResponse.ExpiresIn);

            return response;
        }
    }
    
    public async Task<bool> ConfirmMailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        var user = await CustomIdentityUserManager.TryGetByAsync(x =>
                x.Id.Equals(userId),
            true,
            cancellationToken: cancellationToken
        );

        var decodedToken = Encoding.UTF8.GetString(
            WebEncoders.Base64UrlDecode(token));
        using (CurrentTenant.Change(user!.TenantId))
        {
            var result =
                await IdentityUserManager.ConfirmEmailAsync(user, decodedToken);

            return result.Succeeded;
        }
    }
    
    public async Task<bool> ConfirmChangeEmailAsync(
        Guid userId,
        string newEmail,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        var user = await CustomIdentityUserManager.TryGetByAsync(
            x => x.Id == userId,
            true,
            cancellationToken: cancellationToken
        );
        var decodedToken = WebUtility.UrlDecode(token);
        using (CurrentTenant.Change(user!.TenantId))
        {
            var result = await IdentityUserManager
                .ChangeEmailAsync(user, newEmail, decodedToken);

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
        var user = await CustomIdentityUserManager.TryGetByAsync(x =>
                x.Email == mail, true,
            cancellationToken: cancellationToken);
        using (CurrentTenant.Change(user.TenantId))
        {
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
        }

        return true;
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
        var user = await CustomIdentityUserManager.TryGetByAsync(x =>
                x.Id.Equals(userId), true,
            cancellationToken: cancellationToken);
        var result = await IdentityUserManager.ChangeEmailAsync(user, newMailAddress, token);
        return result.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(
        ChangePasswordInputDto changePasswordInputDto,
        CancellationToken cancellationToken = default
    )
    {
        var userId = CurrentUser.GetId();
        var user = await IdentityUserManager.GetByIdAsync(userId);
        (await IdentityUserManager
            .ChangePasswordAsync(
                user,
                changePasswordInputDto.OldPassword,
                changePasswordInputDto.NewPassword
            )).CheckErrors();
        return true;
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
            Scope = AuthConstants.Scope
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