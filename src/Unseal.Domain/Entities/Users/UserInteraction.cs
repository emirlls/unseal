using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Unseal.Entities.Users;

public class UserInteraction : CreationAuditedEntity<Guid>
{
    public Guid SourceUserId { get; set; }
    public Guid TargetUserId { get; set; }
    
    public bool IsMuted { get; set; }
    public bool IsBlocked { get; set; }
    
    public virtual IdentityUser SourceUser { get; set; }
    public virtual IdentityUser TargetUser { get; set; }

    public UserInteraction(
        Guid id,
        Guid sourceUserId,
        Guid targetUserId
    )
    {
        Id = id;
        SourceUserId = sourceUserId;
        TargetUserId = targetUserId;
    }
}