using System;
using System.Collections.Generic;

namespace Unseal.Dtos.Users;

public class UserDetailDto
{
    public UserDto UserDto { get; set; }
    public List<string>? CapsuleUrls { get; set; }
    public int FollowerCount { get; set; }
    public int FollowCount { get; set; }
    public DateTime LastActivity { get; set; }
}