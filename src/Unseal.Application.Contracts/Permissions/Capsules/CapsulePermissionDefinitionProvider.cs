using Volo.Abp.Authorization.Permissions;

namespace Unseal.Permissions.Capsules;

public class CapsulePermissionDefinitionProvider : UnsealPermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var permissionGroupDefinition = context.AddGroup(CapsulePermissions.GroupName,
            L(CapsulePermissions.GroupName));

        var defaultPermission = permissionGroupDefinition.AddPermission(CapsulePermissions.Default,
            L(CapsulePermissions.Default));
        var createPermission = permissionGroupDefinition.AddPermission(CapsulePermissions.Create,
            L(CapsulePermissions.Create));
        var updatePermission = permissionGroupDefinition.AddPermission(CapsulePermissions.Update,
            L(CapsulePermissions.Update));
        var deletePermission = permissionGroupDefinition.AddPermission(CapsulePermissions.Delete,
            L(CapsulePermissions.Delete));
        var exportPermission = permissionGroupDefinition.AddPermission(CapsulePermissions.Export,
            L(CapsulePermissions.Export));
    }
}