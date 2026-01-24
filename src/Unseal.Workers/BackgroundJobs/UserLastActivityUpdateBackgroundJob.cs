using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using NUglify.Helpers;
using StackExchange.Redis;
using Unseal.Constants;
using Unseal.Extensions;
using Unseal.Interfaces.Managers.Users;
using Unseal.Repositories.Users;
using Volo.Abp.DependencyInjection;

namespace Unseal.Workers.BackgroundJobs;

public class UserLastActivityUpdateBackgroundJob : ITransientDependency
{
    public async Task UpdateLastActivityAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        var userProfileRepository = serviceProvider.GetRequiredService<IUserProfileRepository>();
        var userProfileManager = serviceProvider.GetRequiredService<IUserProfileManager>();
        var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
        var redis = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
        var keys = await redis.GetMembersAsync(CacheConstants.UserActivity.IndexKeyPrefix);
        var userIds = new List<Guid>();
        keys.ForEach(x=>userIds.Add(Guid.Parse(x.ToString().Split(":")[0])));
        var userProfiles = await userProfileManager
            .TryGetListByAsync(x=> userIds.Contains(x.Id), cancellationToken: cancellationToken);
        
        await Parallel.ForEachAsync(userIds, cancellationToken,
            async (userId, ct) =>
            {
                var cacheKeyPrefix = string.Format(CacheConstants.UserActivity.RedisCacheKey, userId);
                var cacheKey = cacheKeyPrefix.GenerateCacheKey();

                var lastActivity = await distributedCache.GetStringAsync(cacheKey, token: cancellationToken);
                if (DateTime.TryParse(lastActivity, out var lastActivityDate))
                {
                    var userProfile = userProfiles.FirstOrDefault(x => x.Id.Equals(userId));
                    userProfile.LastActivityTime = lastActivityDate;
                }
            });
        
        await userProfileRepository.BulkUpdateAsync(userProfiles, cancellationToken);
    }
}