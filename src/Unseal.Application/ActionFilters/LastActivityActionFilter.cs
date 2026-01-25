using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Unseal.Constants;
using Unseal.Extensions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace Unseal.ActionFilters;

public class LastActivityActionFilter : IAsyncActionFilter, ITransientDependency
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        var result = await next();
        var currentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
        if (currentUser is { IsAuthenticated: true, Id: not null })
        {
            var memoryCacheKeyPrefix = string.Format(CacheConstants.UserActivity.MemoryCacheKey, currentUser.GetId());
            var memoryCacheKey = memoryCacheKeyPrefix.GenerateCacheKey();
            var memoryCache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
            var memoryData = memoryCache.TryGetValue(memoryCacheKey, out _);
            // If there is not any data in last 5 minutes, cache the last activity data as datetime now.
            if (!memoryData)
            {
                var redis = context.HttpContext.RequestServices.GetRequiredService<IConnectionMultiplexer>();
                var db = redis.GetDatabase();
                var distributedCache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
                var cacheKeyPrefix = string.Format(CacheConstants.UserActivity.RedisCacheKey, currentUser.GetId());
                var cacheKey = cacheKeyPrefix.GenerateCacheKey();
                var redisValue = DateTime.Now.ToString("O");
                await distributedCache.SetStringAsync(
                    cacheKey,
                    redisValue,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    });
                await db.SetAddAsync(CacheConstants.UserActivity.IndexKeyPrefix, cacheKey);
                memoryCache.Set(memoryCacheKey, DateTime.Now, TimeSpan.FromMinutes(5));
            }
        }
    }
}