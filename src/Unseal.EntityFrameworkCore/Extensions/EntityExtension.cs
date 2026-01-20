using Microsoft.EntityFrameworkCore;

namespace Unseal.Extensions;

public static class EntityExtension
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
}
