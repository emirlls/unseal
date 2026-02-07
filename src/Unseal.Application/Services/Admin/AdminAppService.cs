using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using StackExchange.Redis;
using Unseal.Constants;
using Unseal.Dtos.Admin.Permissions;
using Unseal.Dtos.Admin.Roles;
using Unseal.Localization;
using Unseal.Overrides;
using Unseal.Repositories.Auth;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Unseal.Services.Admin;

public class AdminAppService : ApplicationService, IAdminAppService
{
    private IPermissionDefinitionManager PermissionDefinitionManager
        => LazyServiceProvider.LazyGetRequiredService<IPermissionDefinitionManager>();

    private ICustomRoleRepository CustomRoleRepository =>
        LazyServiceProvider.LazyGetRequiredService<ICustomRoleRepository>();

    private IPermissionGrantRepository PermissionGrantRepository =>
        LazyServiceProvider.LazyGetRequiredService<IPermissionGrantRepository>();

    private IPermissionManager PermissionManager =>
        LazyServiceProvider.LazyGetRequiredService<IPermissionManager>();

    private IdentityRoleManager IdentityRoleManager =>
        LazyServiceProvider.LazyGetRequiredService<IdentityRoleManager>();

    private CustomPermissionStore CustomPermissionStore =>
        LazyServiceProvider.LazyGetRequiredService<CustomPermissionStore>();

    private IConfiguration Configuration =>
        LazyServiceProvider.LazyGetRequiredService<IConfiguration>();

    private IStringLocalizer<UnsealResource> StringLocalizer
        => LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<UnsealResource>>();

    private readonly IStringLocalizerFactory _stringLocalizerFactory;
    private readonly IDataFilter<IMultiTenant> _dataFilter;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    public AdminAppService(
        IStringLocalizerFactory stringLocalizerFactory,
        IDataFilter<IMultiTenant> dataFilter,
        IUnitOfWorkManager unitOfWorkManager
    )
    {
        _stringLocalizerFactory = stringLocalizerFactory;
        _dataFilter = dataFilter;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<List<PermissionGroupDto>?> GetPermissionListAsync(CancellationToken cancellationToken = default)
    {
        var permissionGroups = (await PermissionDefinitionManager
                .GetGroupsAsync())
            .ToList();
        var response = await GetPermissionGroupDtoListAsync(permissionGroups, null);
        return response;
    }

    public async Task<List<PermissionGroupDto>> GetRolePermissionListAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    )
    {
        using (_dataFilter.Disable())
        {
            var rolePermissionNames = (await PermissionGrantRepository.GetListAsync(
                    providerName: RolePermissionValueProvider.ProviderName,
                    providerKey: roleId.ToString(),
                    cancellationToken: cancellationToken
                ))
                .Select(x => x.Name)
                .ToHashSet();

            var allPermissionGroups = (await PermissionDefinitionManager.GetGroupsAsync())
                .ToList();

            var response = await GetPermissionGroupDtoListAsync(allPermissionGroups, rolePermissionNames);
    
            return response;
        }
    }

    public async Task<List<RoleDto>> GetRoleListAsync(CancellationToken cancellationToken = default)
    {
        using (_dataFilter.Disable())
        {
            var roles = await CustomRoleRepository.GetListByAsync(
                x => true,
                asNoTracking: true,
                cancellationToken: cancellationToken
            );
            var roleDtos = new List<RoleDto>();
            foreach (var role in roles)
            {
                roleDtos.Add(new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                });
            }

            return roleDtos;
        }
    }

    private async Task<List<PermissionGroupDto>> GetPermissionGroupDtoListAsync(
        List<PermissionGroupDefinition> permissionGroupDefinitions,
        HashSet<string>? rolePermissionNames
    )
    {
        var permissionGroupDtos = new List<PermissionGroupDto>();
        if (permissionGroupDefinitions.IsNullOrEmpty()) return permissionGroupDtos;

        foreach (var group in permissionGroupDefinitions)
        {
            var filteredPermissionDtos = group.GetPermissionsWithChildren() 
                .WhereIf(!rolePermissionNames.IsNullOrEmpty(),
                    p => rolePermissionNames!.Contains(p.Name))
                .Select(p => new PermissionDto
                {
                    Name = p.Name,
                    LocalizedName = p.DisplayName.Localize(_stringLocalizerFactory)
                })
                .ToList();

            permissionGroupDtos.Add(new PermissionGroupDto
            {
                Name = group.Name,
                Permissions = filteredPermissionDtos
            });
        }

        return permissionGroupDtos;
    }

    public async Task<bool> RoleUpdateAsync(RoleUpdateDto roleUpdateDto, CancellationToken cancellationToken = default)
    {
        using var uow = _unitOfWorkManager.Begin(true);
        using (_dataFilter.Disable())
        {
            var role = await CustomRoleRepository
                .GetByAsync(x => x.Id.Equals(roleUpdateDto.RoleId),
                    cancellationToken: cancellationToken
                );
            if (role is null) throw new UserFriendlyException(StringLocalizer[ExceptionCodes.IdentityRole.NotFound]);
            if (!string.IsNullOrWhiteSpace(roleUpdateDto.RoleName))
            {
                await IdentityRoleManager.SetRoleNameAsync(role, roleUpdateDto.RoleName);
            }

            var rolePermissions = await PermissionGrantRepository.GetListAsync(
                RolePermissionValueProvider.ProviderName,
                role.Id.ToString(),
                cancellationToken: cancellationToken);

            var rolePermissionNames = rolePermissions
                .Select(c => c.Name)
                .ToList();
            var addedPermissions = roleUpdateDto.Permissions
                .Except(rolePermissionNames)
                .ToList();
            var removedPermissions = rolePermissionNames
                .Except(roleUpdateDto.Permissions)
                .ToArray();

            foreach (var newPermission in addedPermissions)
            {
                await PermissionManager.SetForRoleAsync(
                    roleName: role.Id.ToString(),
                    permissionName: newPermission,
                    isGranted: true
                );
            }

            if (removedPermissions.Any())
            {
                var permissionGrants = await PermissionGrantRepository.GetListAsync(
                    removedPermissions,
                    RolePermissionValueProvider.ProviderName,
                    role.Id.ToString(),
                    cancellationToken
                );

                await PermissionGrantRepository.DeleteManyAsync(
                    permissionGrants,
                    cancellationToken: cancellationToken
                );
            }

            await uow.SaveChangesAsync(cancellationToken);
            await uow.CompleteAsync(cancellationToken);

            var redis = await ConnectionMultiplexer.ConnectAsync(
                Configuration[CacheConstants.RedisConfigurationKey]!);
            foreach (var endPoint in redis.GetEndPoints())
            {
                var keys = redis.GetServer(endPoint).Keys(pattern: string.Format(
                    CacheConstants.PermissionGrantCacheItem,
                    UserPermissionValueProvider.ProviderName, role.Id.ToString())).ToArray();
                redis.GetDatabase().KeyDeleteAsync(keys.ToArray()).GetAwaiter().GetResult();
            }

            await CustomPermissionStore.UpdateCacheItemsAsync(
                RolePermissionValueProvider.ProviderName,
                role.Id.ToString()
            );
            return true;
        }
    }
}