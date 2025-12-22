using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Unseal.Extensions;

public static class StringExtension
{
    public static void ToSnakeCase(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(tableName.ConvertToSnakeCase());
            }

            var schema = entity.GetSchema();
            if (!string.IsNullOrEmpty(schema))
            {
                entity.SetSchema(schema.ConvertToSnakeCase());
            }

            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnBaseName(); 
                property.SetColumnName(columnName.ConvertToSnakeCase());
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName().ConvertToSnakeCase());
            }
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(foreignKey.GetConstraintName().ConvertToSnakeCase());
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
