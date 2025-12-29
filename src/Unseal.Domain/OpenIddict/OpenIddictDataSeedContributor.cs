using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using Unseal.Constants;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Unseal.OpenIddict;

public class OpenIddictDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly IConfiguration _configuration;
    
    public OpenIddictDataSeedContributor(
        IConfiguration configuration,
        IOpenIddictApplicationManager applicationManager, 
        IOpenIddictScopeManager scopeManager
    )
    {
        _configuration = configuration;
        _applicationManager = applicationManager;
        _scopeManager = scopeManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await CreateScopeAsync(AuthConstants.Scope);
        await CreateSwaggerApplicationAsync();
    }

    private async Task CreateScopeAsync(string scopeName)
    {
        if (await _scopeManager.FindByNameAsync(scopeName) == null)
        {
            await _scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = scopeName,
                DisplayName = $"{scopeName} API Scope",
                Resources = { AuthConstants.Audience }
            });
        }
    }

    private async Task CreateSwaggerApplicationAsync()
    {
        var clientId = _configuration[AuthConstants.SwaggerClientId];
        var clientSecret = _configuration[AuthConstants.SwaggerClientSecret];

        if (await _applicationManager.FindByClientIdAsync(clientId!) == null)
        {
            await _applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                //ClientSecret = clientSecret,
                DisplayName = "Swagger UI",
                ClientType = OpenIddictConstants.ClientTypes.Public,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "openid",
                    OpenIddictConstants.Permissions.Prefixes.Scope + AuthConstants.Scope,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "email",
                    OpenIddictConstants.Permissions.Prefixes.Scope + "profile",
                    OpenIddictConstants.Permissions.Prefixes.Scope + "roles"
                }
            });
        }
    }
}