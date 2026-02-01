using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Extensions;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Unseal.Configurations.Capsules;

public class CapsuleItemConfiguration : IEntityTypeConfiguration<CapsuleItem>
{
    public void Configure(EntityTypeBuilder<CapsuleItem> builder)
    {
        builder.ToTable(builder.GetTableName(),DatabaseConstants.SchemaName);
        builder.ConfigureByConvention();

        builder.Property(x => x.TextContext).IsRequired(false);
        builder.Property(x => x.FileUrl).IsRequired(false);
        builder.Property(x => x.FileName).IsRequired(false);

        builder.HasOne(x => x.Capsule)
            .WithOne(x => x.CapsuleItems)
            .HasForeignKey<CapsuleItem>(x => x.CapsuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}