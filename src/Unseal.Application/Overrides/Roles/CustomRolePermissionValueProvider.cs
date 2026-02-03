using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unseal.Interfaces.Managers.Auth;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace Unseal.Overrides.Roles;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(RolePermissionValueProvider))]
public class CustomRolePermissionValueProvider : RolePermissionValueProvider
{
    private readonly ICustomIdentityUserManager _customIdentityUserManager;
    private readonly IDataFilter<IMultiTenant> _tenantFilter;

    public CustomRolePermissionValueProvider(
        IPermissionStore permissionStore,
        ICustomIdentityUserManager customIdentityUserManager,
        IDataFilter<IMultiTenant> tenantFilter
    ) : base(permissionStore)
    {
        _customIdentityUserManager = customIdentityUserManager;
        _tenantFilter = tenantFilter;
    }

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return PermissionGrantResult.Undefined;

        var user = await _customIdentityUserManager.TryGetByQueryableAsync(x => x
            .Include(x => x.Roles)
            .Where(x => x.Id.Equals(Guid.Parse(userId))));

        var userRoles = user.Roles;

        if (userRoles.IsNullOrEmpty()) return PermissionGrantResult.Undefined;

        foreach (var userRole in userRoles)
        {
            using (_tenantFilter.Disable())
            {
                if (await PermissionStore.IsGrantedAsync(context.Permission.Name, Name, userRole.RoleId.ToString()))
                {
                    return PermissionGrantResult.Granted;
                }
            }
        }

        return PermissionGrantResult.Undefined;
    }

    public override async Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        var permissionNames = context.Permissions.Select(x => x.Name).Distinct().ToArray();
        var result = new MultiplePermissionGrantResult(permissionNames);

        var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
        if (userId.IsNullOrWhiteSpace() || !Guid.TryParse(userId, out var userGuid)) return result;

        var user = await _customIdentityUserManager.TryGetByQueryableAsync(q => q
            .Include(u => u.Roles)
            .Where(u => u.Id == userGuid));

        if (user?.Roles == null || !user.Roles.Any()) return result;

        var roleIds = user.Roles.Select(r => r.RoleId.ToString())
            .Distinct()
            .ToList();

        foreach (var roleId in roleIds)
        {
            MultiplePermissionGrantResult rolePermissions;
            using (_tenantFilter.Disable())
            {
                rolePermissions = await PermissionStore
                    .IsGrantedAsync(
                        permissionNames,
                        Name,
                        roleId
                    );
            }

            foreach (var (pName, pStatus) in rolePermissions.Result)
            {
                if (result.Result[pName] == PermissionGrantResult.Undefined &&
                    pStatus != PermissionGrantResult.Undefined)
                {
                    result.Result[pName] = pStatus;
                }
            }

            if (result.AllGranted) break;
        }

        return result;
    }
}