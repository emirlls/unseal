using Volo.Abp.Authorization.Permissions;

namespace Unseal.Permissions.Messages;

public class MessagePermissionDefinitionProvider : UnsealPermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var permissionGroupDefinition = context.AddGroup(MessagePermissions.GroupName,
            L(MessagePermissions.GroupName));

        var defaultPermission = permissionGroupDefinition.AddPermission(MessagePermissions.Default,
            L(MessagePermissions.Default));
        var createPermission = permissionGroupDefinition.AddPermission(MessagePermissions.Create,
            L(MessagePermissions.Create));
        var updatePermission = permissionGroupDefinition.AddPermission(MessagePermissions.Update,
            L(MessagePermissions.Update));
        var deletePermission = permissionGroupDefinition.AddPermission(MessagePermissions.Delete,
            L(MessagePermissions.Delete));
        var exportPermission = permissionGroupDefinition.AddPermission(MessagePermissions.Export,
            L(MessagePermissions.Export));
    }
}