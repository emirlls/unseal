using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    ) => await UserAppService.FollowAsync(userId, cancellationToken);
    
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
    ) => await UserAppService.UnfollowAsync(userId, cancellationToken);

    /// <summary>
    /// Use to block user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{userId}/block")]
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
    [HttpGet("{userId}/unblock")]
    public async Task<bool> UnBlockAsync(
        Guid userId,
        CancellationToken cancellationToken = default

    ) => await UserAppService.UnBlockAsync(userId, cancellationToken);
}