namespace Unseal.Dtos.Auth;

public record LoginDto(
    string UserName,
    string Password
);