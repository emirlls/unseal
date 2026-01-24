using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Extensions;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Unseal.Configurations.Capsules;

public class CapsuleConfiguration : IEntityTypeConfiguration<Capsule>
{
    public void Configure(EntityTypeBuilder<Capsule> builder)
    {
        builder.ToTable(builder.GetTableName(),DatabaseConstants.SchemaName);
        builder.ConfigureByConvention();

        builder.Property(x => x.ReceiverId).IsRequired(false);
        builder.Property(x => x.IsOpened).HasDefaultValue(false);

        builder.HasOne(x=>x.CapsuleType)
            .WithMany(x=>x.Capsules)
            .HasForeignKey(x=>x.CapsuleTypeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}