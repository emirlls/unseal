using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unseal.Constants;
using Unseal.Entities.Users;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Users;

public class EfIUserFollowerRepository : EfBaseRepository<UserFollower>, IUserFollowerRepository
{
    public EfIUserFollowerRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<(int followerCount, int followCount)> GetFollowCountsAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();
        var followerCount = await dbSet
            .Where(x => x.UserId.Equals(userId) &&
                        x.StatusId == Guid.Parse(
                            LookupSeederConstants.UserFollowStatusesConstants.Accepted.Id))
            .CountAsync(cancellationToken: cancellationToken);
        
        var followCount = await dbSet
            .Where(x => x.FollowerId.Equals(userId) &&
                        x.StatusId == Guid.Parse(
                            LookupSeederConstants.UserFollowStatusesConstants.Accepted.Id))
            .CountAsync(cancellationToken: cancellationToken);
        return (followerCount, followCount);
    }
}