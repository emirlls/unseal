using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Unseal.Entities.Groups;

public class Group : AuditedEntity<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? GroupImageUrl { get; set; }
    
    public ICollection<GroupMember> GroupMembers { get; set; }

    public Group(Guid id, string name, string description)
    {
        Id=id;
        Name = name;
        Description = description;
    }
}