using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unseal.Entities.Users;
using Unseal.EntityFrameworkCore;
using Unseal.Enums;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Users;

public class EfUserViewTrackingRepository : EfBaseRepository<UserViewTracking>, IUserViewTrackingRepository
{
    public EfUserViewTrackingRepository(IDbContextProvider<UnsealDbContext> dbContextProvider
    ) : base(dbContextProvider)
    {
    }

    public async Task<List<Guid>> UserIdsViewedProfileAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();
        var response = await
            dbSet
                .Include(x => x.UserViewTrackingType)
                .Where(x => x.ExternalId.Equals(currentUserId) &&
                            x.UserViewTrackingType.Code == (int)(UserViewTrackingTypes.UserProfile))
                .Select(x => x.UserId)
                .ToListAsync(cancellationToken: cancellationToken);
        return response;
    }
}