using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unseal.Interfaces.Managers;
using Unseal.Localization;
using Unseal.Repositories.Base;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;

namespace Unseal.Managers;

public class BaseDomainService<TEntity> : DomainService, IBaseDomainService<TEntity>
    where TEntity : class, IEntity<Guid>
{
    private readonly IBaseRepository<TEntity> _baseRepository;
    private readonly IStringLocalizer<UnsealResource> _stringLocalizer;
    private readonly string NotFoundException;
    private readonly string AlreadyExistsException;

    public BaseDomainService(
        IBaseRepository<TEntity> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer,
        string notFoundException,
        string alreadyExistsException
    )
    {
        _baseRepository = baseRepository;
        _stringLocalizer = stringLocalizer;
        NotFoundException = notFoundException;
        AlreadyExistsException = alreadyExistsException;
    }

    public async Task<TEntity?> TryGetByAsync(
        Expression<Func<TEntity, bool>> expression,
        bool throwIfNull = false,
        bool asNoTracking = false,
        bool throwIfExists = false,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _baseRepository.GetByAsync(expression, asNoTracking, cancellationToken)!;
        if (throwIfNull && response is null)
        {
            throw new UserFriendlyException(_stringLocalizer[NotFoundException]);
        }

        if (throwIfExists && response is not null)
        {
            throw new UserFriendlyException(_stringLocalizer[AlreadyExistsException]);
        }

        return response;
    }

    public async Task<List<TEntity>> TryGetListByAsync(
        Expression<Func<TEntity, bool>> expression,
        bool throwIfNull = false,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _baseRepository.GetListByAsync(expression, asNoTracking, cancellationToken);
        if (throwIfNull && response is null)
        {
            throw new UserFriendlyException(_stringLocalizer[NotFoundException]);
        }

        return response;
    }

    public async Task<TEntity> TryGetByQueryableAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        bool throwIfNull = false,
        bool asNoTracking = false,
        bool throwIfExists = false,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _baseRepository
            .TryGetByQueryableAsync(queryBuilder, asNoTracking, cancellationToken);
        
        if (throwIfNull && response is null)
        {
            throw new UserFriendlyException(_stringLocalizer[NotFoundException]);
        }

        if (throwIfExists && response is not null)
        {
            throw new UserFriendlyException(_stringLocalizer[AlreadyExistsException]);
        }

        return response;
    }

    public async Task<IQueryable<TEntity>?> TryGetQueryableAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        bool throwIfNull = false,
        bool asNoTracking = false,
        bool throwIfExists = false,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _baseRepository
            .TryGetQueryableAsync(queryBuilder, asNoTracking, cancellationToken);
        if (throwIfNull && response is null)
        {
            throw new UserFriendlyException(_stringLocalizer[NotFoundException]);
        }

        if (throwIfExists && response is not null)
        {
            throw new UserFriendlyException(_stringLocalizer[AlreadyExistsException]);
        }

        return response;
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _baseRepository.ExistsAsync(expression, cancellationToken);
        return response;
    }
}