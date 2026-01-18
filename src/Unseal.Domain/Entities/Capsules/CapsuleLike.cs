using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Unseal.Entities.Capsules;

public class CapsuleLike : AuditedEntity<Guid>
{
    public Guid UserId {get; set;}
    public Guid CapsuleId {get; set;}
    
    public virtual IdentityUser User {get; set;}
    public virtual Capsule Capsule {get; set;}

    public CapsuleLike(
        Guid id,
        Guid userId,
        Guid capsuleId,
        DateTime creationTime
    )
    {
        Id = id;
        UserId = userId;
        CapsuleId = capsuleId;
        CreationTime = creationTime;
    }
}