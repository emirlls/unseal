using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Volo.Abp.Application.Dtos;

namespace Unseal.Extensions;

public static class FilteringExtension
{
    public static IQueryable<TEntity> ApplyDynamicFilters<TEntity, TFilters>(
        this IQueryable<TEntity> query,
        TFilters filters)
        where TFilters : PagedAndSortedResultRequestDto
    {
        var filterProperties = typeof(TFilters)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var entityType = typeof(TEntity);

        foreach (var prop in filterProperties)
        {
            var value = prop.GetValue(filters);
            if (value == null || string.IsNullOrWhiteSpace(value.ToString())) continue;

            if (new[] { "Sorting", "SkipCount", "MaxResultCount" }
                .Contains(prop.Name)) continue;

            // 2. Özel Durum: FilterText (Genel Arama)
            if (prop.Name == "FilterText")
            {
                // Entity içinde string olan ve sık kullanılan kolonlarda ara
                var searchFields = new[] { "Name", "Description", "Title", "Code" };
                var searchableProps = entityType.GetProperties()
                    .Where(p => p.PropertyType == typeof(string) && searchFields.Contains(p.Name))
                    .Select(p => p.Name);

                if (searchableProps.Any())
                {
                    var predicate = string.Join(" || ", searchableProps.Select(name => $"{name}.Contains(@0)"));
                    query = query.Where(predicate, value);
                }
                continue;
            }

            if (prop.Name.EndsWith("Min") || prop.Name.EndsWith("Max"))
            {
                var isMin = prop.Name.EndsWith("Min");
                var realPropName = prop.Name.Substring(0, prop.Name.Length - 3);
                
                if (entityType.GetProperty(realPropName) != null)
                {
                    var comparison = isMin ? ">=" : "<=";
                    query = query.Where($"{realPropName} {comparison} @0", value);
                    continue;
                }
            }

            var targetProp = entityType.GetProperty(prop.Name);
            if (targetProp != null)
            {
                if (targetProp.PropertyType == typeof(string))
                {
                    query = query.Where($"{prop.Name}.Contains(@0)", value);
                }
                else
                {
                    query = query.Where($"{prop.Name} == @0", value);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(filters.Sorting))
        {
            query = query.OrderBy(filters.Sorting);
        }

        return query;
    }
}