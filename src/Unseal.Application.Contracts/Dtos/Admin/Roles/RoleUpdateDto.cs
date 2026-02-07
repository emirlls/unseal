using System;
using System.Collections.Generic;

namespace Unseal.Dtos.Admin.Roles;

public class RoleUpdateDto
{
    public Guid RoleId { get; set; }
    public string? RoleName { get; set; }
    public List<string> Permissions { get; set; }
}