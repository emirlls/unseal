using System;
using System.Collections.Generic;
using Unseal.Entities.Lookups;
using Unseal.Entities.Users;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Unseal.Entities.Capsules;

public class Capsule : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid? ReceiverId { get; set; }
    public string Name { get; set; }
    public bool? IsOpened { get; set; }
    public DateTime RevealDate { get; set; }
    public Guid? CapsuleTypeId { get; set; }

    public virtual CapsuleType CapsuleType { get; set; }
    public virtual ICollection<CapsuleItem> CapsuleItems { get; set; }
    public virtual ICollection<CapsuleMapFeature> CapsuleMapFeatures { get; set; }
    public virtual ICollection<CapsuleComment> CapsuleComments { get; set; }
    public virtual ICollection<CapsuleLike> CapsuleLikes { get; set; }

    public Capsule(
        Guid id,
        Guid? tenantId,
        Guid? receiverId,
        Guid? capsuleTypeId,
        Guid? creatorId,
        string name,
        DateTime revealDate
    )
    {
        Id = id;
        TenantId = tenantId;
        ReceiverId = receiverId;
        Name = name;
        RevealDate = revealDate;
        CapsuleTypeId = capsuleTypeId;
        CreatorId = creatorId;
    }
}