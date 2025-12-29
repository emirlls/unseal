using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;

namespace Unseal.Entities.Users;

public class UserFollower : Entity<Guid>
{
    public Guid UserId { get; set; }
    public Guid FollowerId { get; set; }
    
    public bool IsMuted { get; set; }
    public bool IsBlocked { get; set; }
    public virtual IdentityUser User { get; set; }
    public virtual IdentityUser Follower { get; set; }

    public UserFollower(Guid id, Guid userId, Guid followerId)
    {
        Id = id;
        UserId = userId;
        FollowerId = followerId;
    }
}