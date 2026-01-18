using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unseal.Attributes;

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
        bool isDescending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        var prop = typeof(TFilter).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (prop == null) return query;

        var mappingAttr = prop.GetCustomAttribute<FilterMappedAttribute>();
        string dbFieldName = mappingAttr != null ? mappingAttr.MapTo : fieldName;

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