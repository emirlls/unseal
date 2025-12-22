using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Unseal.Constants;
using Unseal.Dtos.Auth;
using Unseal.Interfaces.Managers.Auth;
using Unseal.Profiles.Auth;
using Volo.Abp;
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
        return result.Succeeded;
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginDto loginDto,
        CancellationToken cancellationToken = default
    )
    {
        var tokenResponse = await GetTokenAsync(
            loginDto.UserName,
            loginDto.Password,
            cancellationToken
        );
        
        var user = await CustomIdentityUserManager.TryGetByAsync(x=>
            string.Equals(x.UserName, loginDto.UserName), 
            cancellationToken: cancellationToken);
        
        var response = new LoginResponseDto(
            user.Id,
            tokenResponse.AccessToken,
            tokenResponse.RefreshToken,
            tokenResponse.ExpiresIn);
        
        return response;
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