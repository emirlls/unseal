using Unseal.Constants;

namespace Unseal.Permissions.Admin;

public class AdminPermissions
{
    public const string GroupName = "Admin";
    public const string Default = $"{GroupName}.{PermissionTypes.Default}";

    public static class Users
    {
        public const string Delete = $"{GroupName}.Users.{PermissionTypes.Delete}";
    }
}