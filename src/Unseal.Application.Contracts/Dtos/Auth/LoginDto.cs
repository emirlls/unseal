namespace Unseal.Dtos.Auth;

public record LoginDto(
    string Email,
    string Password
);