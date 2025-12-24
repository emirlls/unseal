using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Unseal.Entities.Capsules;

public class CapsuleItem : FullAuditedAggregateRoot<Guid>
{
    public Guid CapsuleId { get; set; }
    public string ContentType { get; set; }
    public string? TextContext { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    
    public virtual Capsule Capsule { get; set; }

    public CapsuleItem(
        Guid id,
        Guid capsuleId,
        string contentType,
        string textContext,
        string fileUrl,
        string fileName
    )
    {
        Id = id;
        CapsuleId = capsuleId;
        ContentType = contentType;
        TextContext = textContext;
        FileUrl = fileUrl;
        FileName = fileName;
    }
}