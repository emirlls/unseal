using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unseal.Constants;
using Unseal.Entities.Lookups;
using Unseal.Extensions;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Unseal.Configurations.Lookups;

public class UserViewTrackingTypeConfiguration : IEntityTypeConfiguration<UserViewTrackingType>
{
    public void Configure(EntityTypeBuilder<UserViewTrackingType> builder)
    {
        builder.ToTable(builder.GetTableName(),DatabaseConstants.SchemaName);
        builder.ConfigureByConvention();
    }
}