using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unseal.Constants;
using Unseal.Entities.Notifications;
using Unseal.Extensions;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Unseal.Configurations.Lookups;

public class NotificationEventTypeConfiguration : IEntityTypeConfiguration<NotificationEventType>
{
    public void Configure(EntityTypeBuilder<NotificationEventType> builder)
    {
        builder.ToTable(builder.GetTableName(),DatabaseConstants.SchemaName);
        builder.ConfigureByConvention();
    }
}