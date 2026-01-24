using System;

namespace Unseal.Dtos.Capsules;

public record CapsuleDto(
    Guid Id,
    Guid CreatorId,
    string Name,
    string? Type,
    string Username,
    string? ProfilePictureUrl,
    DateTime RevealDate,
    DateTime CreationTime
    );