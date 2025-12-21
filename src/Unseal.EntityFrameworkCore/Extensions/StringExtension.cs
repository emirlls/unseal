using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Unseal.Extensions;

public static class StringExtension
{
    public static void ToSnakeCase(this ModelBuilder builder)
    {
        var entityTypes = builder.Model.GetEntityTypes();
        foreach (var entityType in entityTypes)
        {
            if (entityType.IsOwned())
            {
                continue;
            }
            entityType.SetTableName(entityType.GetTableName()?.ConvertToSnakeCase());
            entityType.SetSchema(entityType.GetSchema()?.ConvertToSnakeCase());
            var entityProperties = entityType.GetProperties().ToList();
            foreach (var propertyName in entityProperties.Select(c => c.Name))
            {
                builder.Entity(entityType.Name,
                    entityTypeBuilder =>
                        entityTypeBuilder.Property(propertyName).HasColumnName(propertyName.ConvertToSnakeCase()));
            }
        }
    }
    public static string ConvertToSnakeCase(this string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        if (name.Contains('_') && name.All(c => char.IsLower(c) || c == '_'))
        {
            return name;
        }

        // ex: UserId -> _User_Id
        var snakeCase = Regex.Replace(
            name, 
            "([A-Z])", 
            "_$1", 
            RegexOptions.Compiled
        );

        // ex: _User_Id -> User_Id
        snakeCase = snakeCase.Trim('_');

        // ex: User__ID -> User_ID
        snakeCase = Regex.Replace(snakeCase, "_+", "_");
        
        return snakeCase.ToLowerInvariant();
    }
}
