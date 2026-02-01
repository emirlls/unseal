using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Unseal.Entities.Users;

public class UserViewTracking : AuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid CapsuleId { get; set; }

    public UserViewTracking(
        Guid id,
        Guid userId,
        Guid capsuleId,
        DateTime creationTime
    )
    {
        Id= id;
        UserId = userId;
        CapsuleId = capsuleId;
        CreationTime =  creationTime;
    }
}