using Unseal.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Unseal.Permissions;

public class UnsealPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(UnsealPermissions.GroupName, L("Permission:Unseal"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<UnsealResource>(name);
    }
}
