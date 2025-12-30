using System;

namespace Unseal.Dtos.Users;

public record GroupDto(
    Guid Id,
    string Name,
    string Description,
    string? GroupImageUrl
);
