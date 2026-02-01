using System;

namespace Unseal.Models.ServerSentEvents;

public class CapsuleCreatedEventModel
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Name { get; set; }
    public string? Username { get; set; }
    public string? FileUrl { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime RevealDate { get; set; }
    public DateTime CreationTime { get; set; }
}