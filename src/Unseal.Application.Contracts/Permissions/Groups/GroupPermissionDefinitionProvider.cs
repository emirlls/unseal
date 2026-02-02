using Volo.Abp.Authorization.Permissions;

namespace Unseal.Permissions.Groups;

public class GroupPermissionDefinitionProvider : UnsealPermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var permissionGroupDefinition = context.AddGroup(GroupPermissions.GroupName,
            L(GroupPermissions.GroupName));

        var defaultPermission = permissionGroupDefinition.AddPermission(GroupPermissions.Default,
            L(GroupPermissions.Default));
        var createPermission = permissionGroupDefinition.AddPermission(GroupPermissions.Create,
            L(GroupPermissions.Create));
        var updatePermission = permissionGroupDefinition.AddPermission(GroupPermissions.Update,
            L(GroupPermissions.Update));
        var deletePermission = permissionGroupDefinition.AddPermission(GroupPermissions.Delete,
            L(GroupPermissions.Delete));
        var exportPermission = permissionGroupDefinition.AddPermission(GroupPermissions.Export,
            L(GroupPermissions.Export));
    }
}