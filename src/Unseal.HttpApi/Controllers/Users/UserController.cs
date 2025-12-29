using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unseal.Services.Users;
using Volo.Abp.DependencyInjection;

namespace Unseal.Controllers.Users;

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

    [HttpPost("follow")]
    public async Task<bool> FollowAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await UserAppService.FollowAsync(userId, cancellationToken);
    }
    
    [HttpPost("unfollow")]
    public async Task<bool> UnfollowAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await UserAppService.UnfollowAsync(userId, cancellationToken);
    }
}