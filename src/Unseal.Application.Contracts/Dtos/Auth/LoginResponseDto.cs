using System;

namespace Unseal.Dtos.Auth;

public class LoginResponseDto
{
    public Guid UserId { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public int ExpireIn { get; set; }
    public bool IsActive { get; set; } = true;
}