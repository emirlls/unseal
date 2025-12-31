using System;

namespace Unseal.Dtos.Groups;

public record GroupDto(
    Guid Id,
    string Name,
    string Description,
    string? GroupImageUrl
);
