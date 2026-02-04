using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unseal.Entities.Capsules;
using Unseal.EntityFrameworkCore;
using Unseal.Models.Dashboards;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Capsules;

public class EfCapsuleRepository : EfBaseRepository<Capsule>, ICapsuleRepository
{
    public EfCapsuleRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : 
        base(dbContextProvider)
    {
    }

    public async Task<List<string?>> GetCapsuleUrlsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();
        var response = await dbSet
            .Where(x => x.CreatorId.Equals(userId))
            .Include(x=>x.CapsuleItems)
            .Select(x=>x.CapsuleItems.FileUrl)
            .ToListAsync(cancellationToken: cancellationToken);
        return response;
    }

    public async Task<List<CreationCapsuleByDateModel>> GetCreationCapsuleByDateAsync(
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();
        var start = (startDate ?? DateTime.Now.AddMonths(-1)).Date;
        var end = (endDate ?? DateTime.Now).Date;

        var dbData = await dbSet
            .Where(x => x.CreationTime >= start && x.CreationTime < end.AddDays(1))
            .Select(x => x.CreationTime)
            .ToListAsync(cancellationToken);

        var groupedDbData = dbData
            .GroupBy(d => d.Date)
            .ToDictionary(g => g.Key, g => g.Count());

        var response = Enumerable.Range(0, (end - start).Days + 1)
            .Select(offset => start.AddDays(offset))
            .Select(day => new CreationCapsuleByDateModel
            {
                Day = day.ToString("yyyy-MM-dd"),
                Count = groupedDbData.ContainsKey(day) ? groupedDbData[day] : 0
            })
            .ToList();

        return response;
    }
}