using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unseal.Constants;
using Unseal.Entities.Notifications;
using Unseal.Extensions;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Unseal.Configurations.Notifications;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable(builder.GetTableName(),DatabaseConstants.SchemaName);
        builder.ConfigureByConvention();
        
        builder.HasOne(x=>x.NotificationEventType)
            .WithMany(x=>x.Notifications)
            .HasForeignKey(x=>x.NotificationEventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsRead).HasDefaultValue(false);
    }
}