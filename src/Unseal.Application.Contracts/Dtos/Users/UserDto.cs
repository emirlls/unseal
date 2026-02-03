using System;

namespace Unseal.Dtos.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? ProfilePictureUrl { get; set; }
}