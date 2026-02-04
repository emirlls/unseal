using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Dtos.Admin.Permissions;
using Unseal.Dtos.Admin.Roles;
using Volo.Abp.Application.Services;

namespace Unseal.Services.Admin;

public interface IAdminAppService : IApplicationService
{
    Task<List<PermissionGroupDto>>? GetPermissionListAsync(
        CancellationToken cancellationToken = default
    );
    
    Task<List<PermissionGroupDto>>? GetRolePermissionListAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    );
    
    Task<List<RoleDto>>? GetRoleListAsync(
        CancellationToken cancellationToken = default
    );
    
    Task<bool> RoleUpdateAsync(
        RoleUpdateDto roleUpdateDto,
        CancellationToken cancellationToken = default
    );
}