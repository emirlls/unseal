using System;
using System.Collections.Generic;

namespace Unseal.Dtos.Capsules;

public class CapsuleDetailDto
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string? CreatorUserName { get; set; }
    public string Name { get; set; }
    public string? Type { get; set; }
    public string? CreatorProfilePictureUrl { get; set; }
    public string? FileUrl { get; set; }
    public DateTime RevealDate  { get; set; }
    public DateTime CreationTime { get; set; }
    public List<CapsuleLikeDto> LikeDtos { get; set; }
    public List<CapsuleCommentDto> CommentDtos { get; set; }
}