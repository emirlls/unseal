using System;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Entities.Users;

namespace Unseal.Interfaces.Managers.Users;

public interface IUserFollowerManager : IBaseDomainService<UserFollower>
{
    UserFollower Create(Guid userId, Guid followerId);

    Task<bool> CheckUserBlockedAsync(
        Guid? userId,
        Guid followerId,
        CancellationToken cancellationToken = default
    );
}