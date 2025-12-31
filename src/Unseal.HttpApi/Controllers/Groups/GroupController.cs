using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unseal.Dtos.Groups;
using Unseal.Filtering.Users;
using Unseal.Services.Groups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace Unseal.Controllers.Groups;

[ApiController]
[Route("api/group")]
public class GroupController : UnsealController
{
    private readonly IAbpLazyServiceProvider _abpLazyServiceProvider;

    public GroupController(IAbpLazyServiceProvider abpLazyServiceProvider)
    {
        _abpLazyServiceProvider = abpLazyServiceProvider;
    }
    private IGroupAppService GroupAppService =>
        _abpLazyServiceProvider.LazyGetRequiredService<IGroupAppService>();

    /// <summary>
    /// Use to filtered group list.
    /// </summary>
    /// <param name="groupFilters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<PagedResultDto<GroupDto>> GetFilteredGroupListAsync(
        [FromQuery]GroupFilters groupFilters,
        CancellationToken cancellationToken = default
    )
    {
        return await GroupAppService.GetFilteredGroupListAsync(
            groupFilters,
            cancellationToken
        );
    }
    
    /// <summary>
    /// Use to group detail.
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{groupId}")]
    public async Task<GroupDetailDto> GetDetailAsync(
        Guid groupId,
        CancellationToken cancellationToken = default
    )
    {
        return await GroupAppService.GetDetailAsync(groupId, cancellationToken);
    }
    
    /// <summary>
    /// Use to create group
    /// </summary>
    /// <param name="groupCreateDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> CreateGroupAsync(
        [FromForm]GroupCreateDto groupCreateDto,
        CancellationToken cancellationToken = default)
    {
        return await GroupAppService.CreateGroupAsync(
            groupCreateDto, 
            cancellationToken
        );
    }

    /// <summary>
    /// Use to update group
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="groupUpdateDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{groupId}")]
    public async Task<bool> UpdateGroupAsync(
        Guid groupId,
        GroupUpdateDto groupUpdateDto,
        CancellationToken cancellationToken = default)
    {
        return await GroupAppService.UpdateGroupAsync(
            groupId,
            groupUpdateDto, 
            cancellationToken
        );
    }

    /// <summary>
    /// Use to leave the group
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{groupId}/leave")]
    public async Task<bool> LeaveAsync(
        Guid groupId,
        CancellationToken cancellationToken = default
    )
    {
        return await GroupAppService.LeaveAsync(groupId,cancellationToken);
    }
    
}