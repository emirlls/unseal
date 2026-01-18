using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unseal.Attributes;
using Unseal.Filtering.Base;

namespace Unseal.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> ApplyDynamicFilters<TEntity, TFilter>(
        this IQueryable<TEntity> query, 
        TFilter? request) 
        where TFilter : DynamicFilterRequest
    {
        if (request == null || request.Filters == null || !request.Filters.Any()) 
            return query;

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        Expression? combinedExpression = null;

        foreach (var filter in request.Filters)
        {
            if (string.IsNullOrEmpty(filter.Prop) || string.IsNullOrEmpty(filter.Value)) continue;

            var propInfo = typeof(TFilter).GetProperty(filter.Prop, 
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
            var mappingAttr = propInfo?.GetCustomAttribute<FilterMappedAttribute>();
            string dbFieldName = mappingAttr != null ? mappingAttr.MapTo : filter.Prop;

            var filterExpression = FilterStrategyExecutor.BuildExpression(parameter, new FilterItem 
            { 
                Prop = dbFieldName,
                Strategy = filter.Strategy,
                Value = filter.Value 
            });

            combinedExpression = combinedExpression == null 
                ? filterExpression 
                : Expression.AndAlso(combinedExpression, filterExpression);
        }

        return combinedExpression != null 
            ? query.Where(Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter)) 
            : query;
    }
}