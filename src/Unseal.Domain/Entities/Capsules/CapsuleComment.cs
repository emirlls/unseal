using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Unseal.Entities.Capsules;

public class CapsuleComment : AuditedEntity<Guid>
{
    public Guid CapsuleId { get; set; }
    public Guid UserId { get; set; }
    public string Comment { get; set; }
    
    public virtual Capsule Capsule { get; set; }
    public virtual IdentityUser User { get; set; }

    public CapsuleComment(
        Guid id,
        Guid capsuleId, 
        Guid userId,
        string comment,
        DateTime creationTime
    )
    {
        Id = id;
        CapsuleId = capsuleId;
        UserId = userId;
        Comment = comment;
        CreationTime = creationTime;
    }
}