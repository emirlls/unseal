using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unseal.Constants;
using Unseal.Entities.Users;
using Unseal.Extensions;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Unseal.Configurations.Users;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable(builder.GetTableName(),DatabaseConstants.SchemaName);
        builder.ConfigureByConvention();
        
        builder.Property(g => g.GroupImageUrl).IsRequired(false);
    }
}