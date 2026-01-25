using System;
using Unseal.Entities.Lookups;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Unseal.Entities.Messages;

public class ChatMessage : AuditedEntity<Guid>
{
    public Guid SenderId { get; set; }
    public Guid TargetId { get; set; }
    public Guid ChatTypeId { get; set; }
    public string Content { get; set; }
    
    public virtual ChatType ChatType { get; set; }
    public  virtual IdentityUser SenderUser { get; set; }

    public ChatMessage(
        Guid id,
        Guid senderId,
        Guid targetId, 
        Guid chatTypeId,
        string content,
        DateTime creationTime
    )
    {
        Id= id;
        SenderId = senderId;
        TargetId = targetId;
        ChatTypeId = chatTypeId;
        Content = content;
        CreationTime = creationTime;
    }
}