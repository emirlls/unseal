using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Unseal.Repositories.Base;

public interface IBaseRepository<TEntity> : IRepository<TEntity>, ITransientDependency
    where TEntity : class, IEntity<Guid>
{
    Task<TEntity?> GetByAsync(
        Expression<Func<TEntity, bool>> expression,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>?> GetListByAsync(
        Expression<Func<TEntity, bool>> expression,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );

    Task<TEntity?> TryGetQueryableAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> TryGetListQueryableAsync(
        IQueryable<TEntity> queryable,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    Task HardDeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken = default
    );
    Task BulkInsertAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    );

    Task BulkUpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    );
    Task HardDeleteManyAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    );
}