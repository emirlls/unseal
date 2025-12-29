using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Unseal.Services.Users;

public interface IUserAppService : IApplicationService
{
    Task<bool> FollowAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    Task<bool> UnfollowAsync(
        Guid userId, 
        CancellationToken cancellationToken = default
    );
}