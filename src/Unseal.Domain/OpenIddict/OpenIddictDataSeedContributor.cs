using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Json;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;

namespace Unseal.OpenIddict;

public class OpenIddictDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IOpenIddictApplicationRepository _applicationRepository;
    private readonly IOpenIddictScopeRepository _scopeRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IConfiguration _configuration;
    
    private readonly IJsonSerializer _jsonSerializer;
    private const string DefaultScopeName = "Unseal";
    
    public OpenIddictDataSeedContributor(
        IOpenIddictApplicationRepository applicationRepository,
        IOpenIddictScopeRepository scopeRepository,
        IGuidGenerator guidGenerator, 
        IConfiguration configuration,
        IJsonSerializer jsonSerializer)
    {
        _applicationRepository = applicationRepository;
        _scopeRepository = scopeRepository;
        _guidGenerator = guidGenerator;
        _configuration = configuration;
        _jsonSerializer = jsonSerializer;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await CreateScopeAsync(DefaultScopeName);
        var clientId = _configuration["AuthServer:SwaggerClientId"] ?? "Unseal_Swagger";
        await CreateSwaggerApplicationAsync(clientId);
    }

    private async Task CreateScopeAsync(string scopeName)
    {
        if (await _scopeRepository.FindByNameAsync(scopeName) != null) return;

        await _scopeRepository.InsertAsync(new OpenIddictScope(_guidGenerator.Create())
        {
            Name = scopeName,
            DisplayName = $"{scopeName} API Scope",
            Resources = _jsonSerializer.Serialize(new List<string> { scopeName })
        });
    }

    private async Task CreateSwaggerApplicationAsync(string clientId)
    {
        if (await _applicationRepository.FindByClientIdAsync(clientId) != null) return;

        var permissions = new List<string>
        {
            "endpoint:token",
            "grant_type:password",
            "grant_type:refresh_token",
            "scope:Unseal",
            "scope:address",
            "scope:email",
            "scope:offline_access",
            "scope:phone",
            "scope:profile",
            "scope:roles"
        };

        await _applicationRepository.InsertAsync(new OpenIddictApplication(_guidGenerator.Create())
        {
            ClientId = clientId,
            ClientType = "public",
            DisplayName = "Swagger UI",
            Permissions = _jsonSerializer.Serialize(permissions)
        });
    }
}