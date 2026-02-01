using System;

namespace Unseal.Models.Capsules;

public class CapsuleBaseModel
{
    public Guid? CapsuleTypeId { get; set; }
    public Guid? ReceiverId { get; set; }
    public string? Name { get; set; }
    public string? ContentType { get; set; }
    public string? TextContext { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? GeoJson { get; set; }
    public DateTime RevealDate { get; set; }
}