using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Dtos.Auth;
using Unseal.Etos;
using Unseal.Interfaces.Managers.Auth;
using Unseal.Localization;
using Unseal.Profiles.Auth;
using Volo.Abp;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;

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
    public async Task<bool> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var model = AuthMapper.MapToModel(dto);
        
        var user =await CustomIdentityUserManager.Create(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            model
        );
        var result = await IdentityUserManager.CreateAsync(user, model.Password);
        result.CheckErrors();
        await IdentityUserManager.AddDefaultRolesAsync(user);
        
        var  mailConfirmationToken = await IdentityUserManager
            .GenerateEmailConfirmationTokenAsync(user);

        var userRegisterEto = new UserRegisterEto
        {
            UserId = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            ConfirmationToken = mailConfirmationToken
        };
        await DistributedEventBus.PublishAsync(userRegisterEto);
        return result.Succeeded;
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginDto loginDto,
        CancellationToken cancellationToken = default
    )
    {
        var user = await CustomIdentityUserManager.TryGetByAsync(x=>
                string.Equals(x.UserName, loginDto.UserName), 
            throwIfNull:true,
            cancellationToken: cancellationToken);
        
        var tokenResponse = await GetTokenAsync(
            loginDto.UserName,
            loginDto.Password,
            cancellationToken
        );
        
        var response = new LoginResponseDto(
            user.Id,
            tokenResponse.AccessToken,
            tokenResponse.RefreshToken,
            tokenResponse.ExpiresIn);
        
        return response;
    }

    public async Task<bool> ConfirmMailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        var user = await CustomIdentityUserManager.TryGetByAsync(x=>
            x.Id.Equals(userId) ,
            throwIfNull:true,
            cancellationToken: cancellationToken
        );
        var result = await IdentityUserManager
            .ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<bool> UserDeleteAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var user = await IdentityUserManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            throw new UserFriendlyException(StringLocalizer[ExceptionCodes.IdentityUser.NotFound]);
        }
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

    private async Task<TokenResponse> GetTokenAsync(
        string username,
        string password,
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
        }, cancellationToken: cancellationToken);
        
        var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = discovery.TokenEndpoint,
            ClientId = Configuration[AuthConstants.SwaggerClientId]!,
            ClientSecret = Configuration[AuthConstants.SwaggerClientSecret]!,
            UserName = username,
            Password = password,
            Scope = AuthConstants.Scope
        }, cancellationToken: cancellationToken);

        if (tokenResponse.IsError)
        {
            throw new UserFriendlyException(tokenResponse.Error!);
        }
    
        return tokenResponse;
    }
}