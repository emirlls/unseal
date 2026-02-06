using System;
using Unseal.Entities.Lookups;
using Volo.Abp.Domain.Entities.Auditing;

namespace Unseal.Entities.Users;

public class UserViewTracking : AuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid ExternalId { get; set; } // viewed capsule or user id.
    public Guid? UserViewTrackingTypeId { get; set; } // capsule or user profile.
    
    public virtual UserViewTrackingType  UserViewTrackingType { get; set; }

    public UserViewTracking(
        Guid id,
        Guid userId,
        Guid externalId,
        Guid? userViewTrackingTypeId,
        DateTime creationTime
    )
    {
        Id= id;
        UserId = userId;
        ExternalId = externalId;
        UserViewTrackingTypeId = userViewTrackingTypeId;
        CreationTime =  creationTime;
    }
}