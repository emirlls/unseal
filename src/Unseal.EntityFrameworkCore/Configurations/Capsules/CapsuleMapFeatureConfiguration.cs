using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Extensions;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Unseal.Configurations.Capsules;

public class CapsuleMapFeatureConfiguration : IEntityTypeConfiguration<CapsuleMapFeature>
{
    public void Configure(EntityTypeBuilder<CapsuleMapFeature> builder)
    {
        builder.ToTable(builder.GetTableName(),DatabaseConstants.SchemaName);
        builder.ConfigureByConvention();
        
        builder.HasOne(x=>x.Capsule)
            .WithMany(x=>x.CapsuleMapFeatures)
            .HasForeignKey(x=>x.CapsuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}