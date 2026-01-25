using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;

namespace Unseal.Interfaces.Managers;

public interface IBaseDomainService<TEntity> : IDomainService
    where TEntity : class, IEntity<Guid>
{
    Task<TEntity?> TryGetByAsync(
        Expression<Func<TEntity, bool>> expression,
        bool throwIfNull = false,
        bool asNoTracking = false,
        bool throwIfExists = false,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> TryGetListByAsync(
        Expression<Func<TEntity, bool>> expression,
        bool throwIfNull = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );

    Task<TEntity> TryGetByQueryableAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        bool throwIfNull = false,
        bool asNoTracking = false,
        bool throwIfExists = false,
        CancellationToken cancellationToken = default
    );
    Task<List<TEntity>> TryGetListByQueryableAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        bool throwIfNull = false,
        bool asNoTracking = false,
        bool throwIfExists = false,
        CancellationToken cancellationToken = default
    );
    Task<IQueryable<TEntity>?> TryGetQueryableAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        bool throwIfNull = false,
        bool asNoTracking = false,
        bool throwIfExists = false,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default
    );
}