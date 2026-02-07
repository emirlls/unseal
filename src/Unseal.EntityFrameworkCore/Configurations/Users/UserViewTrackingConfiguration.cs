using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unseal.Constants;
using Unseal.Entities.Users;
using Unseal.Extensions;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Unseal.Configurations.Users;

public class UserViewTrackingConfiguration : IEntityTypeConfiguration<UserViewTracking>
{
    public void Configure(EntityTypeBuilder<UserViewTracking> builder)
    {
        builder.ToTable(builder.GetTableName(),DatabaseConstants.SchemaName);
        builder.ConfigureByConvention();
        
        builder.HasOne(x=>x.UserViewTrackingType)
            .WithMany(x=>x.UserViewTrackings)
            .HasForeignKey(x=>x.UserViewTrackingTypeId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder
            .Property(x => x.UserViewTrackingTypeId)
            .IsRequired(false);
    }
}