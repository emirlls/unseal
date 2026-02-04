using Unseal.Constants;

namespace Unseal.Permissions.Admin;

public class AdminPermissions
{
    public const string GroupName = "Admin";
    public const string Default = $"{GroupName}.{PermissionTypes.Default}";
}