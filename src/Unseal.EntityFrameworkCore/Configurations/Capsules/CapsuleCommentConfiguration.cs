using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Extensions;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Unseal.Configurations.Capsules;

public class CapsuleCommentConfiguration : IEntityTypeConfiguration<CapsuleComment>
{
    public void Configure(EntityTypeBuilder<CapsuleComment> builder)
    {
        builder.ToTable(builder.GetTableName(),DatabaseConstants.SchemaName);
        builder.ConfigureByConvention();

        builder.HasOne(x=>x.Capsule)
            .WithMany(x=>x.CapsuleComments)
            .HasForeignKey(x=>x.CapsuleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}