using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;

namespace Unseal.Entities.Groups;

public class GroupMember : Entity<Guid>
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinDate { get; set; }

    public virtual Group? Group { get; set; }
    public virtual IdentityUser? User { get; set; }
    
    public GroupMember(Guid id, Guid groupId, Guid userId)
    {
        Id = id;
        GroupId = groupId;
        UserId = userId;
    }
}