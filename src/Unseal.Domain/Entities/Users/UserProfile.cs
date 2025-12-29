using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Unseal.Entities.Users;

public class UserProfile : AuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    public bool IsLocked { get; set; }
    public bool AllowJoinGroup { get; set; }
    public DateTime LastActivity { get; set; }
    
    public virtual IdentityUser User { get; set; }

    public UserProfile(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
        LastActivity = DateTime.Now;
    }
}