using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unseal.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

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

        var query = DynamicQueryableExtensions.Where(dbSet, expression);
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
}