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
        IQueryable<TEntity> queryable,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );
    
    Task<List<TEntity>> TryGetListQueryableAsync(
        IQueryable<TEntity> queryable,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );
}