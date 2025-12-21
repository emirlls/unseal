using Volo.Abp.Reflection;

namespace Unseal.Permissions;

public class UnsealPermissions
{
    public const string GroupName = "Unseal";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(UnsealPermissions));
    }
}
