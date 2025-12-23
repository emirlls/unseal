using System;
using System.Collections.Generic;
using Unseal.Entities.Lookups;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Unseal.Entities.Capsules;

public class Capsule : FullAuditedAggregateRoot<Guid>,IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid? ReceiverId { get; set; }
    public string Name { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsOpened { get; set; }
    public DateTime RevealDate { get; set; }
    
    public Guid? CapsuleTypeId { get; set; }
    
    public virtual CapsuleType CapsuleType { get; set; }
    public virtual ICollection<CapsuleItem> CapsuleItems { get; set; }

    public Capsule(
        Guid id,
        Guid? tenantId,
        Guid? receiverId,
        string name,
        bool? isPublic,
        DateTime revealDate,
        Guid? capsuleTypeId)
    {
        
    }
}