using System;
using System.Collections.Generic;
using Unseal.Entities.Capsules;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Unseal.Entities.Users;

public class UserProfile : AuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    public bool IsLocked { get; set; }
    public bool AllowJoinGroup { get; set; }
    public string? Content { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime LastActivityTime { get; set; }
    
    public virtual IdentityUser User { get; set; }
    public virtual ICollection<Capsule>  Capsules { get; set; }
    public UserProfile(
        Guid id,
        Guid userId,
        string? content,
        string? profilePictureUrl,
        DateTime creationTime
    )
    {
        Id = id;
        UserId = userId;
        Content = content;
        ProfilePictureUrl = profilePictureUrl;
        LastActivityTime = DateTime.Now;
        CreationTime = creationTime;
    }
}