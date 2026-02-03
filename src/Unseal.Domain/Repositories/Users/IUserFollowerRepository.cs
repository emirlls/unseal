using System;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Entities.Users;
using Unseal.Repositories.Base;

namespace Unseal.Repositories.Users;

public interface IUserFollowerRepository : IBaseRepository<UserFollower>
{
    Task<(int followerCount, int followCount)> GetFollowCountsAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
}