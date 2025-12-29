using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unseal.Dtos.Users;
using Unseal.Services.Users;
using Volo.Abp.DependencyInjection;

namespace Unseal.Controllers.Users;

[Authorize]
[ApiController]
[Route("api/user")]
public class UserController : UnsealController
{
    private readonly IAbpLazyServiceProvider _abpLazyServiceProvider;

    public UserController(IAbpLazyServiceProvider abpLazyServiceProvider)
    {
        _abpLazyServiceProvider = abpLazyServiceProvider;
    }
    
    private IUserAppService UserAppService =>
    _abpLazyServiceProvider.LazyGetRequiredService<IUserAppService>();

    /// <summary>
    /// Use to follow the user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("follow")]
    public async Task<bool> FollowAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await UserAppService.FollowAsync(userId, cancellationToken);
    }
    
    /// <summary>
    /// Use to unfollow the user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("unfollow")]
    public async Task<bool> UnfollowAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await UserAppService.UnfollowAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Use to create group
    /// </summary>
    /// <param name="groupCreateDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("group")]
    public async Task<bool> CreateGroupAsync(
        [FromForm]GroupCreateDto groupCreateDto,
        CancellationToken cancellationToken = default)
    {
        return await UserAppService.CreateGroupAsync(
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
    [HttpPut("group")]
    public async Task<bool> UpdateGroupAsync(
        Guid groupId,
        GroupUpdateDto groupUpdateDto,
        CancellationToken cancellationToken = default)
    {
        return await UserAppService.UpdateGroupAsync(
            groupId,
            groupUpdateDto, 
            cancellationToken
        );
    }
}