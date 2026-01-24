using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unseal.Entities.Users;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Users;

public class EfUserProfileRepository : EfBaseRepository<UserProfile>, IUserProfileRepository
{
    public EfUserProfileRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<Dictionary<Guid,string?>> GetProfilePictureUrlsByUserIdsAsync(HashSet<Guid> userId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var response = await dbSet
            .Where(x => userId.Contains(x.UserId))
            .ToDictionaryAsync(
                x => x.UserId,
                x => x.ProfilePictureUrl,
                cancellationToken: cancellationToken
            );
        return response;
    }
}