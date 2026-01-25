using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.SimpleStateChecking;

namespace Unseal.Overrides.Permissions;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(PermissionChecker), typeof(IPermissionChecker))]
public class CustomPermissionChecker : PermissionChecker, IPermissionChecker
{
    private readonly IDataFilter<IMultiTenant> _tenantFilter;

    public CustomPermissionChecker(
        ICurrentPrincipalAccessor principalAccessor,
        IPermissionDefinitionManager permissionDefinitionManager,
        ICurrentTenant currentTenant,
        IPermissionValueProviderManager permissionValueProviderManager,
        ISimpleStateCheckerManager<PermissionDefinition> stateCheckerManager,
        IDataFilter<IMultiTenant> tenantFilter
    ) : base(
        principalAccessor,
        permissionDefinitionManager,
        currentTenant,
        permissionValueProviderManager,
        stateCheckerManager
    )
    {
        _tenantFilter = tenantFilter;
    }


    public override async Task<bool> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, string name)
    {
        var permission = await PermissionDefinitionManager.GetOrNullAsync(name);
        if (permission is not { IsEnabled: true })
        {
            return false;
        }

        if (!await StateCheckerManager.IsEnabledAsync(permission))
        {
            return false;
        }

        var context = new PermissionValueCheckContext(permission, claimsPrincipal);
        var isGranted = false;

        foreach (var provider in PermissionValueProviderManager.ValueProviders)
        {
            if (!permission.Providers.IsNullOrEmpty() && !permission.Providers.Contains(provider.Name))
            {
                continue;
            }

            using (_tenantFilter.Disable())
            {
                var result = await provider.CheckAsync(context);

                if (result == PermissionGrantResult.Prohibited)
                {
                    return false;
                }

                if (result == PermissionGrantResult.Granted)
                {
                    isGranted = true;
                }
            }
        }

        return isGranted;
    }

    public override async Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names)
    {
        return await IsGrantedAsync(PrincipalAccessor.Principal, names);
    }

    public override async Task<MultiplePermissionGrantResult> IsGrantedAsync(
        ClaimsPrincipal? claimsPrincipal,
        string[] names
    )
    {
        var result = new MultiplePermissionGrantResult(names);

        if (names.IsNullOrEmpty())
        {
            return result;
        }

        var validPermissionDefinitions = new List<PermissionDefinition>();
        foreach (var name in names)
        {
            var permission = await PermissionDefinitionManager.GetOrNullAsync(name);
            if (permission != null && permission.IsEnabled && await StateCheckerManager.IsEnabledAsync(permission))
            {
                validPermissionDefinitions.Add(permission);
            }
            else
            {
                result.Result[name] = PermissionGrantResult.Prohibited;
            }
        }

        if (validPermissionDefinitions.IsNullOrEmpty())
        {
            return result;
        }

        var context = new PermissionValuesCheckContext(
            validPermissionDefinitions,
            claimsPrincipal
        );

        foreach (var provider in PermissionValueProviderManager.ValueProviders)
        {
            var filteredPermissions = validPermissionDefinitions
                .Where(p => p.Providers.Count == 0 || p.Providers.Contains(provider.Name))
                .ToList();

            if (filteredPermissions.IsNullOrEmpty())
            {
                continue;
            }

            MultiplePermissionGrantResult providerResult;

            using (_tenantFilter.Disable())
            {
                providerResult = await provider.CheckAsync(context);
            }

            foreach (var permission in filteredPermissions)
            {
                var permissionName = permission.Name;
                if (result.Result[permissionName] == PermissionGrantResult.Undefined &&
                    providerResult.Result.TryGetValue(permissionName, out var permissionGrantResult) &&
                    permissionGrantResult != PermissionGrantResult.Undefined)
                {
                    result.Result[permissionName] = permissionGrantResult;
                }
            }

            validPermissionDefinitions
                .RemoveAll(p => result.Result[p.Name] != PermissionGrantResult.Undefined);

            if (validPermissionDefinitions.IsNullOrEmpty() || result.AllGranted || result.AllProhibited)
            {
                break;
            }
        }

        return result;
    }
}