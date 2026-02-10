using System;

namespace Unseal.Dtos.Capsules;

public class CapsuleCommentDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string Comment { get; set; }
    public string? UserProfilePictureUrl { get; set; }
}