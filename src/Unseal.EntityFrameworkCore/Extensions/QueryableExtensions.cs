using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unseal.Attributes;
using Unseal.Constants;
using Unseal.Filtering.Base;
using Volo.Abp;

namespace Unseal.Extensions;

public static class QueryableExtensions
{
    private static readonly HashSet<string> SupportedStrategies = new()
    {
        FilterOperators.Equals,
        FilterOperators.NotEquals,
        FilterOperators.GreaterThan,
        FilterOperators.GreaterThanOrEqual,
        FilterOperators.LessThan,
        FilterOperators.LessThanOrEqual,
        FilterOperators.Contains
    };

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

            // Only properties declared with [FilterMapped] on the filter type can be queried.
            var dbFieldName = GetFilterMappedFieldName(typeof(TFilter), filter.Prop)
                              ?? throw CreateFilterException(ExceptionCodes.Filter.InvalidProperty);

            if (filter.Strategy is null || !SupportedStrategies.Contains(filter.Strategy))
            {
                throw CreateFilterException(ExceptionCodes.Filter.InvalidStrategy);
            }

            Expression filterExpression;
            try
            {
                filterExpression = FilterStrategyExecutor.BuildExpression(parameter, new FilterItem
                {
                    Prop = dbFieldName,
                    Strategy = filter.Strategy,
                    Value = filter.Value
                });
            }
            catch (Exception e) when (e is InvalidOperationException or NotSupportedException)
            {
                // The strategy is not applicable to the field type (e.g. Contains on a Guid).
                throw CreateFilterException(ExceptionCodes.Filter.InvalidStrategy);
            }
            catch (Exception e) when (e is ArgumentException or FormatException or InvalidCastException
                                          or OverflowException)
            {
                throw CreateFilterException(ExceptionCodes.Filter.InvalidValue);
            }

            combinedExpression = combinedExpression == null
                ? filterExpression
                : Expression.AndAlso(combinedExpression, filterExpression);
        }

        return combinedExpression != null
            ? query.Where(Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter))
            : query;
    }

    internal static string? GetFilterMappedFieldName(Type filterType, string prop)
    {
        var propInfo = filterType.GetProperty(prop,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        return propInfo?.GetCustomAttribute<FilterMappedAttribute>()?.MapTo;
    }

    internal static UserFriendlyException CreateFilterException(string code)
    {
        // The code is mapped to 400 Bad Request and localized by the exception handling pipeline.
        return new UserFriendlyException(code, code);
    }
}
