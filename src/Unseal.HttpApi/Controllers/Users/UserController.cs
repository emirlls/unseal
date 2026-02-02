using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unseal.Dtos.Users;
using Unseal.Permissions.Users;
using Unseal.Services.Users;
using Volo.Abp.Application.Dtos;
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
    /// Use to follow or send follow request to user.
    /// If user account is public, directly follow
    /// else send pending follow request.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("follow")]
    [Authorize(UserPermissions.Default)]
    public async Task<bool> FollowAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    ) => await UserAppService.FollowAsync(userId, cancellationToken);
    
    /// <summary>
    /// Use to unfollow the user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("unfollow")]
    [Authorize(UserPermissions.Default)]
    public async Task<bool> UnfollowAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    ) => await UserAppService.UnfollowAsync(userId, cancellationToken);
    /// <summary>
    /// Use to block user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("block")]
    [Authorize(UserPermissions.Default)]
    public async Task<bool> BlockAsync(
        Guid userId,
        CancellationToken cancellationToken = default

    ) => await UserAppService.BlockAsync(userId, cancellationToken);
    
    /// <summary>
    /// Use to unblock user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("unblock")]
    [Authorize(UserPermissions.Default)]
    public async Task<bool> UnBlockAsync(
        Guid userId,
        CancellationToken cancellationToken = default

    ) => await UserAppService.UnBlockAsync(userId, cancellationToken);

    /// <summary>
    /// Use to accept follow request.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("accept-follow-request")]
    [Authorize(UserPermissions.Default)]
    public async Task<bool> AcceptFollowRequestAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    ) => await UserAppService.AcceptFollowRequestAsync(userId, cancellationToken);
    
    /// <summary>
    /// Use to follow request to user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("reject-follow-request")]
    [Authorize(UserPermissions.Default)]
    public async Task<bool> RejectFollowRequestAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    ) => await UserAppService.RejectFollowRequestAsync(userId, cancellationToken);

    /// <summary>
    /// Use to search users.
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("search")]
    [Authorize(UserPermissions.Default)]
    public async Task<PagedResultDto<UserDto>> SearchAsync(
        string userName, 
        CancellationToken cancellationToken = default
    ) => await UserAppService.SearchAsync(userName, cancellationToken);
    
    /// <summary>
    /// Use to list follow requests.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("follow-requests")]
    [Authorize(UserPermissions.Default)]
    public async Task<PagedResultDto<UserDto>> GetFollowRequestsAsync(
        CancellationToken cancellationToken = default
    ) => await UserAppService.GetFollowRequestsAsync(cancellationToken);
    
    /// <summary>
    /// Use to list my followers.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("followers")]
    [Authorize(UserPermissions.Default)]
    public async Task<PagedResultDto<UserDto>> GetFollowersAsync(
        CancellationToken cancellationToken = default
    ) => await UserAppService.GetFollowersAsync(cancellationToken);

    
    /// <summary>
    /// Use to get user profile
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{userId}/profile")]
    [Authorize(UserPermissions.Default)]
    public async Task<UserDetailDto> GetProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    ) => await UserAppService.GetProfileAsync(userId, cancellationToken);
    
    /// <summary>
    /// Use to get blocked users.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("blocked-users")]
    [Authorize(UserPermissions.Default)]
    public async Task<PagedResultDto<UserDto>> GetBlockedUsersAsync(
        CancellationToken cancellationToken = default
    ) => await UserAppService.GetBlockedUsersAsync(cancellationToken);

    /// <summary>
    /// Use to user profile.
    /// </summary>
    /// <param name="userProfileUpdateDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{userId}/profile")]
    [Authorize(UserPermissions.Update)]
    public async Task<bool> UpdateProfileAsync(
        [FromForm] UserProfileUpdateDto userProfileUpdateDto,
        CancellationToken cancellationToken = default
    ) => await UserAppService.UpdateProfileAsync(
        userProfileUpdateDto, 
        cancellationToken
    );
}