using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unseal.Entities.Capsules;
using Unseal.EntityFrameworkCore;
using Unseal.Extensions;
using Unseal.Filtering.Capsules;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Capsules;

public class EfCapsuleRepository : EfBaseRepository<Capsule>, ICapsuleRepository
{
    public EfCapsuleRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : 
        base(dbContextProvider)
    {
    }

    public async Task<List<Capsule>> GetFilteredListAsync(
        CapsuleFilters capsuleFilters,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();
        var query = await dbSet
            .ApplyDynamicFilters(capsuleFilters)
            .OrderBy(x=>x.RevealDate)
            .ToListAsync(cancellationToken: cancellationToken);

        return query;
    }

    public async Task<long> GetFilteredCountAsync(
        CapsuleFilters capsuleFilters,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();
        var count = await dbSet
            .ApplyDynamicFilters(capsuleFilters)
            .OrderBy(x=>x.RevealDate)
            .CountAsync(cancellationToken: cancellationToken);
        return count;
    }
}