using System.Collections.Generic;

namespace Unseal.Dtos.Admin.Permissions;

public class PermissionGroupDto
{
    public string Name { get; set; }
    public List<PermissionDto> Permissions { get; set; }
}