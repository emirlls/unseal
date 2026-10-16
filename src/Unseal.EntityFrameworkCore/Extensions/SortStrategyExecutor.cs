using System;
using System.Linq;
using System.Linq.Expressions;
using Unseal.Constants;

namespace Unseal.Extensions;

public static class SortStrategyExecutor
{
    public static IQueryable<TEntity> ApplySorting<TEntity, TFilter>(
        IQueryable<TEntity> query, 
        string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting)) return query;

        var parts = sorting.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var fieldName = parts[0];
        if (parts.Length > 2 ||
            (parts.Length == 2 &&
             !parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase) &&
             !parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase)))
        {
            throw QueryableExtensions.CreateFilterException(ExceptionCodes.Filter.InvalidSorting);
        }
        bool isDescending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        // Only properties declared with [FilterMapped] on the filter type can be sorted.
        var dbFieldName = QueryableExtensions.GetFilterMappedFieldName(typeof(TFilter), fieldName)
                          ?? throw QueryableExtensions.CreateFilterException(ExceptionCodes.Filter.InvalidSorting);

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        Expression property = parameter;
        
        foreach (var member in dbFieldName.Split('.'))
        {
            property = Expression.Property(property, member);
        }

        var lambda = Expression.Lambda(property, parameter);
        string methodName = isDescending ? "OrderByDescending" : "OrderBy";
        
        var resultExpression = Expression.Call(
            typeof(Queryable), 
            methodName, 
            new Type[] { typeof(TEntity), property.Type },
            query.Expression, 
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<TEntity>(resultExpression);
    }
}