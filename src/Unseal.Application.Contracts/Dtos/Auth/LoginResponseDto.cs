using System;

namespace Unseal.Dtos.Auth;

public record LoginResponseDto(
    Guid UserId, 
    string? AccessToken,
    string? RefreshToken,
    int ExpiresIn
);