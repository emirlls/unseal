namespace Unseal.Dtos.Auth;

public class ChangePasswordInputDto
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}