using System;
using NetTopologySuite.Geometries;
using Volo.Abp.Domain.Entities.Auditing;

namespace Unseal.Entities.Capsules;

public class CapsuleMapFeature : AuditedEntity<Guid>
{
    public Guid CapsuleId { get; set; }
    public Geometry Geom { get; set; }
    
    public virtual Capsule Capsule { get; set; }

    public CapsuleMapFeature(Guid id,Guid capsuleId, DateTime creationTime)
    {
        Id = id;
        CapsuleId = capsuleId;
        CreationTime = creationTime;
    }
}