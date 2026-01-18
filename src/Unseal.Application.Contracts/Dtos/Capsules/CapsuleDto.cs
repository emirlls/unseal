using System;

namespace Unseal.Dtos.Capsules;

public record CapsuleDto(
    Guid Id,
    string Name,
    string? Type,
    DateTime RevealDate
    );