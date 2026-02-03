using Unseal.Constants;

namespace Unseal.Permissions.Messages;

public class MessagePermissions
{
    public const string GroupName = "Message";
    public const string Default = $"{GroupName}.{PermissionTypes.Default}";
    public const string Create = $"{GroupName}.{PermissionTypes.Create}";
    public const string Update = $"{GroupName}.{PermissionTypes.Update}";
    public const string Delete = $"{GroupName}.{PermissionTypes.Delete}";
    public const string Export = $"{GroupName}.{PermissionTypes.Export}";
}