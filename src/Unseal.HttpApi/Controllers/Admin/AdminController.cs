using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unseal.Dtos.Admin.Permissions;
using Unseal.Dtos.Admin.Roles;
using Unseal.Dtos.Dashboards;
using Unseal.Permissions.Admin;
using Unseal.Services.Admin;
using Unseal.Services.Dashboards;

namespace Unseal.Controllers.Admin;

[Authorize(AdminPermissions.Default)]
[ApiController]
[Route("admin")]
public class AdminController : UnsealController
{
    private IAdminAppService AdminAppService =>
        LazyServiceProvider.LazyGetRequiredService<IAdminAppService>();
    private IDashboardAppService DashboardAppService =>
        LazyServiceProvider.LazyGetRequiredService<IDashboardAppService>();
    
    /// <summary>
    /// Use to role list.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("roles")]
    public async Task<List<RoleDto>> GetRoleListAsync(
        CancellationToken cancellationToken = default
    ) =>  await AdminAppService.GetRoleListAsync(cancellationToken);
    
    /// <summary>
    /// Use to permission list.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("permissions")]
    public async Task<List<PermissionGroupDto>>GetPermissionListAsync(
        CancellationToken cancellationToken = default
    ) => await AdminAppService.GetPermissionListAsync(cancellationToken);
    
    /// <summary>
    /// Use to list permissions of role.
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{roleId}/permissions")]
    public async  Task<List<PermissionGroupDto>> GetRolePermissionListAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    ) => await AdminAppService.GetRolePermissionListAsync(
        roleId, 
        cancellationToken
    );
    
    /// <summary>
    /// Get application usage by time dashboard.
    /// </summary>
    /// <param name="endDate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    [HttpPost("usage-by-time")]
    public async Task<DashboardDto> GetUsageByTimeAsync(
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default
    ) => await DashboardAppService.GetUsageByTimeAsync(
        startDate,
        endDate,
        cancellationToken
    );

    /// <summary>
    /// Get create capsule by time dashboard.
    /// </summary>
    /// <param name="endDate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    [HttpPost("creation-capsule-by-time")]
    public async Task<DashboardDto> GetCreationCapsuleByTimeAsync(
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default
    ) => await DashboardAppService.GetCreationCapsuleByTimeAsync(
        startDate,
        endDate,
        cancellationToken
    );
    
    /// <summary>
    /// Use to update role.
    /// </summary>
    /// <param name="roleUpdateDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("roles")]
    public async Task<bool> RoleUpdateAsync(
        RoleUpdateDto roleUpdateDto,
        CancellationToken cancellationToken = default
    ) => await AdminAppService.RoleUpdateAsync(
        roleUpdateDto, 
        cancellationToken
    );

}