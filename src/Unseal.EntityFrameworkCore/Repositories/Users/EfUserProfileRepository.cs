using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unseal.Entities.Users;
using Unseal.EntityFrameworkCore;
using Unseal.Models.Dashboards;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Users;

public class EfUserProfileRepository : EfBaseRepository<UserProfile>, IUserProfileRepository
{
    public EfUserProfileRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<Dictionary<Guid, string?>> GetProfilePictureUrlsByUserIdsAsync(
        HashSet<Guid> userId,
        CancellationToken cancellationToken = default
    )
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

    public async Task<List<UsageByTimeModel>>  GetLastActivityByDateAsync(
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();
        var start = (startDate ?? DateTime.Now.AddMonths(-1)).Date;
        var end = (endDate ?? DateTime.Now).Date;

        var dbData = await dbSet
            .Where(x => x.LastActivityTime >= start && x.LastActivityTime < end.AddDays(1))
            .Select(x => x.LastActivityTime.Date)
            .ToListAsync(cancellationToken);

        var groupedDbData = dbData
            .GroupBy(d => d.Date)
            .ToDictionary(g => g.Key, g => g.Count());

        var response = Enumerable.Range(0, (end - start).Days + 1)
            .Select(offset => start.AddDays(offset))
            .Select(day => new UsageByTimeModel
            {
                Day = day.ToString("yyyy-MM-dd"),
                Count = groupedDbData.ContainsKey(day) ? groupedDbData[day] : 0
            })
            .ToList();

        return response;
    }
}