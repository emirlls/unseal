using Volo.Abp.Authorization.Permissions;

namespace Unseal.Permissions.Admin;

public class AdminPermissionDefinitionProvider : UnsealPermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var permissionGroupDefinition = context.AddGroup(AdminPermissions.GroupName,
            L(AdminPermissions.GroupName));

        var defaultPermission = permissionGroupDefinition.AddPermission(AdminPermissions.Default,
            L(AdminPermissions.Default));
    }
}