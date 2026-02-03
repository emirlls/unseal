using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unseal.Entities.Capsules;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Capsules;

public class EfCapsuleRepository : EfBaseRepository<Capsule>, ICapsuleRepository
{
    public EfCapsuleRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : 
        base(dbContextProvider)
    {
    }

    public async Task<List<string?>> GetCapsuleUrlsByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var response = await dbSet
            .Where(x => x.CreatorId.Equals(userId))
            .Include(x=>x.CapsuleItems)
            .Select(x=>x.CapsuleItems.FileUrl)
            .ToListAsync(cancellationToken: cancellationToken);
        return response;
    }
}