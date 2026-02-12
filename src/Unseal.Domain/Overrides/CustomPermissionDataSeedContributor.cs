using System;
using System.Linq;
using System.Threading.Tasks;
using Unseal.Repositories.Auth;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Unseal.Overrides;

[Dependency(ReplaceServices = true)]
[ExposeServices(
    typeof(PermissionDataSeedContributor),
    typeof(IDataSeedContributor),
    typeof(CustomPermissionDataSeedContributor)
)]
public class CustomPermissionDataSeedContributor : PermissionDataSeedContributor
{
    private readonly ICustomRoleRepository _customRoleRepository;
    private readonly IDataFilter<IMultiTenant> _dataFilter;

    public CustomPermissionDataSeedContributor(IPermissionDefinitionManager permissionDefinitionManager,
        IPermissionDataSeeder permissionDataSeeder, ICurrentTenant currentTenant,
        ICustomRoleRepository customRoleRepository, IDataFilter<IMultiTenant> dataFilter) : base(
        permissionDefinitionManager,
        permissionDataSeeder, currentTenant)
    {
        _customRoleRepository = customRoleRepository;
        _dataFilter = dataFilter;
    }

    public override async Task SeedAsync(DataSeedContext context)
    {
        var multiTenancySide = CurrentTenant.GetMultiTenancySide();
        var permissionNames = (await PermissionDefinitionManager.GetPermissionsAsync())
            .Where(p => p.MultiTenancySide.HasFlag(multiTenancySide))
            .Where(p => !p.Providers.Any() || p.Providers.Contains(RolePermissionValueProvider.ProviderName))
            .Select(p => p.Name)
            .ToArray();


        using (_dataFilter.Disable())
        {
            var superAdminRole = await _customRoleRepository.GetByAsync(x =>
                !x.TenantId.HasValue &&
                x.IsStatic &&
                !x.IsPublic &&
                x.Name == "admin");
        
            await PermissionDataSeeder.SeedAsync(
                RolePermissionValueProvider.ProviderName,
                superAdminRole.Id.ToString(),
                permissionNames,
                context?.TenantId
            );
        }
    }
}