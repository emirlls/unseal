using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Unseal.Entities.Notifications;

public class NotificationTemplate : FullAuditedEntity<Guid>
{
    public string Content { get; set; }
    public string Culture { get; set; }
    public string? Subject { get; set; }
    public string AppName { get; set; }
    public Guid NotificationEventTypeId { get; set; }
    
    public virtual NotificationEventType NotificationEventType { get; set; }
    
    public NotificationTemplate()
    {
        
    }
}