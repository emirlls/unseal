using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Unseal.Entities.Notifications;

public class Notification : FullAuditedAggregateRoot<Guid>
{
    public Guid DestinationUserId { get; set; }
    public Guid NotificationEventId { get; set; }
    public bool IsRead { get; set; }

    public virtual NotificationEventType NotificationEventType { get; set; }
    
    public Notification(DateTime creationTime)
    {
        CreationTime = creationTime;
    }
}