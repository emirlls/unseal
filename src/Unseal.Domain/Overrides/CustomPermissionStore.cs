using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.PermissionManagement;

namespace Unseal.Overrides;

public class CustomPermissionStore : PermissionStore
{
    public CustomPermissionStore(IPermissionGrantRepository permissionGrantRepository,
        IDistributedCache<PermissionGrantCacheItem> cache,
        IPermissionDefinitionManager permissionDefinitionManager) : base(permissionGrantRepository, cache,
        permissionDefinitionManager)
    {
    }

    public async Task UpdateCacheItemsAsync(
        string providerName,
        string providerKey
    )
    {
        await SetCacheItemsAsync(
            providerName, 
            providerKey,
            "",
            new PermissionGrantCacheItem(false)
        );
    }

    public async Task<string?> GetCacheKeyAsync(
        string permissionName,
        string providerName, 
        string providerKey
    )
    {
        var key = CalculateCacheKey(
            permissionName,
            providerName,
            providerKey);

        return key;
    }
}