using Volo.Abp.Authorization.Permissions;

namespace Unseal.Permissions.Users;

public class UserPermissionDefinitionProvider : UnsealPermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var permissionGroupDefinition = context.AddGroup(UserPermissions.GroupName,
            L(UserPermissions.GroupName));

        var defaultPermission = permissionGroupDefinition.AddPermission(UserPermissions.Default,
            L(UserPermissions.Default));
        var createPermission = permissionGroupDefinition.AddPermission(UserPermissions.Create,
            L(UserPermissions.Create));
        var updatePermission = permissionGroupDefinition.AddPermission(UserPermissions.Update,
            L(UserPermissions.Update));
        var deletePermission = permissionGroupDefinition.AddPermission(UserPermissions.Delete,
            L(UserPermissions.Delete));
        var exportPermission = permissionGroupDefinition.AddPermission(UserPermissions.Export,
            L(UserPermissions.Export));
    }
}