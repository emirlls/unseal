using System;
using Unseal.Entities.Lookups;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Unseal.Entities.Users;

public class UserFollower : AuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid FollowerId { get; set; }
    public Guid StatusId { get; set; }
    
    public virtual IdentityUser User { get; set; }
    public virtual IdentityUser Follower { get; set; }
    public virtual UserFollowStatus Status { get; set; }
    public UserFollower(
        Guid id,
        Guid userId,
        Guid followerId,
        Guid statusId,
        DateTime creationTime)
    {
        Id = id;
        UserId = userId;
        FollowerId = followerId;
        StatusId = statusId;
        CreationTime = creationTime;
    }
}