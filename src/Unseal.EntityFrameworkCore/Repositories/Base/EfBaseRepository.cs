using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Unseal.EntityFrameworkCore;
using Unseal.Extensions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Users;

namespace Unseal.Repositories.Base;

public class EfBaseRepository<TEntity> : EfCoreRepository<UnsealDbContext, TEntity, Guid>,
    IBaseRepository<TEntity>
    where TEntity : class, IEntity<Guid>
{
    public EfBaseRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<TEntity?> GetByAsync(
        Expression<Func<TEntity, bool>> expression,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();

        var query = dbSet.Where(expression);
        if (asNoTracking)
        {
            query = query
                .AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(expression, cancellationToken: cancellationToken);
    }

    public async Task<List<TEntity>?> GetListByAsync(
        Expression<Func<TEntity, bool>> expression,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();

        var query = DynamicQueryableExtensions.Where(dbSet, expression);
        if (asNoTracking)
        {
            query = query
                .AsNoTracking();
        }

        return await query
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<TEntity?> TryGetQueryableAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = await GetQueryableAsync();
        query = queryBuilder(query);
        if (asNoTracking)
        {
            query = query
                .AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<TEntity>> TryGetListQueryableAsync(
        IQueryable<TEntity> queryable,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<TEntity> query = queryable;

        if (asNoTracking)
        {
            query = query
                .AsNoTracking();
        }

        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();
        var response = await dbSet
            .AnyAsync(predicate, cancellationToken: cancellationToken);
        return response;
    }

    public async Task HardDeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        using (DataFilter.Disable<ISoftDelete>())
        {
            var dbSet = await GetDbSetAsync();
            var dbContext = await GetDbContextAsync();
            dbSet.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task BulkInsertAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    )
    {
        var dbContext = await GetDbContextAsync();
        await dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
    }

    public async Task BulkUpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    )
    {
        var dbContext = await GetDbContextAsync();
        dbContext.Set<TEntity>().UpdateRange(entities);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task HardDeleteManyAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    )
    {
        using (DataFilter.Disable<ISoftDelete>())
        {
            var dbSet = await GetDbSetAsync();
            var dbContext = await GetDbContextAsync();
            dbSet.RemoveRange(entities);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<TEntity>> GetDynamicListAsync<TFilters>(
        TFilters filters,
        CancellationToken cancellationToken = default

    ) where TFilters : PagedAndSortedResultRequestDto
    {
        var dbSet = await GetDbSetAsync();
        var currentUser = ServiceProvider.GetRequiredService<ICurrentUser>();
        var distributedCache = ServiceProvider.GetRequiredService<IDistributedCache>();
        var cacheCountKey = CacheExtension.GenerateCacheKeyToCount(
            CultureInfo.CurrentCulture,
            currentUser.Id,
            typeof(TFilters).Name
        );
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(int.MaxValue)
        };
        var response= dbSet
            .ApplyDynamicFilters(filters)
            .AsQueryable();
        var count = await response.CountAsync(cancellationToken: cancellationToken);
        await distributedCache.SetStringAsync(
            cacheCountKey,
            count.ToString(),
            cacheOptions,
            token: cancellationToken);
        return await response
            .PageBy(filters.SkipCount, filters.MaxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> GetDynamicListCountAsync<TFilters>(
        TFilters filters,
        CancellationToken cancellationToken = default

    ) where TFilters : PagedAndSortedResultRequestDto
    {
        var currentUser = ServiceProvider.GetRequiredService<ICurrentUser>();
        var distributedCache = ServiceProvider.GetRequiredService<IDistributedCache>();
        var cacheCountKey = CacheExtension.GenerateCacheKeyToCount(
            CultureInfo.CurrentCulture,
            currentUser.Id,
            typeof(TFilters).Name
        );
        var count = await distributedCache.GetStringAsync(
            cacheCountKey,
            token: cancellationToken
        );
        if (count == null) return -1;
        return Convert.ToInt32(count);
    }
}