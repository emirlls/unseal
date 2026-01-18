using System;
using System.Globalization;
using System.Linq.Expressions;
using Unseal.Constants;
using Unseal.Filtering.Base;

namespace Unseal.Extensions;

public static class FilterStrategyExecutor
{
    public static Expression BuildExpression(
        ParameterExpression param,
        FilterItem filterItem
    )
    {

        Expression property = param;
        foreach (var member in filterItem.Prop.Split('.'))
        {
            property = Expression.Property(property, member);
        }

        var propType = property.Type;
        var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;

        // Date time
        if (underlyingType == typeof(DateTime))
        {
            if (!DateTime.TryParse(filterItem.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue))
                throw new ArgumentException($"Invalid date format: {filterItem.Value}");

            var startOfDay = dateValue.Date;
            var endOfDay = dateValue.Date.AddDays(1).AddTicks(-1);

            return filterItem.Strategy switch
            {
                FilterOperators.Equals => Expression.AndAlso(
                    Expression.GreaterThanOrEqual(property, Expression.Constant(startOfDay, propType)),
                    Expression.LessThanOrEqual(property, Expression.Constant(endOfDay, propType))),
                FilterOperators.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, Expression.Constant(startOfDay, propType)),
                FilterOperators.LessThanOrEqual => Expression.LessThanOrEqual(property, Expression.Constant(endOfDay, propType)),
                FilterOperators.GreaterThan => Expression.GreaterThan(property, Expression.Constant(endOfDay, propType)),
                FilterOperators.LessThan => Expression.LessThan(property, Expression.Constant(startOfDay, propType)),
                _ => throw new NotSupportedException($"{filterItem.Strategy} is not supported for date time.")
            };
        }

        // Other types
        var convertedValue = ConvertValue(filterItem.Value, underlyingType);
        var constant = Expression.Constant(convertedValue, propType);

        return filterItem.Strategy switch
        {
            FilterOperators.Equals => Expression.Equal(property, constant),
            FilterOperators.NotEquals => Expression.NotEqual(property, constant),
            FilterOperators.GreaterThan => Expression.GreaterThan(property, constant),
            FilterOperators.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
            FilterOperators.LessThan => Expression.LessThan(property, constant),
            FilterOperators.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
            FilterOperators.Contains => BuildContainsExpression(property, filterItem.Value),
            _ => Expression.Equal(property, constant)
        };
    }

    private static Expression BuildContainsExpression(Expression property, string? value)
    {
        if (property.Type != typeof(string))
            throw new InvalidOperationException("Contains can use only for string fields.");

        var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        var constant = Expression.Constant(value, typeof(string));

        return Expression.Call(property, method!, constant);
    }

    private static object? ConvertValue(string? value, Type targetType)
    {
        if (string.IsNullOrEmpty(value)) return null;

        if (targetType.IsEnum) return Enum.Parse(targetType, value, true);

        if (targetType == typeof(bool)) return value.ToLower() is "true" or "1";

        return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
    }
}