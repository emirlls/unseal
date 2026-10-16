using System;

namespace Unseal.Dtos.Auth;

public class ConfirmPasswordResetDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
