using System;

namespace Unseal.Dtos.Capsules;

public record CapsuleDto(
    Guid Id,
    Guid CreatorId,
    string Name,
    string? Type,
    string? Username,
    string? ProfilePictureUrl,
    string? FileUrl,
    DateTime RevealDate,
    DateTime CreationTime
    );