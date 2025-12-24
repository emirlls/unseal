namespace Unseal.Constants;

public static class ExceptionCodes
{
    private const string ExceptionCodePrefix = "Unseal:ExceptionCodes:";
    public const string UnexpectedException = $"{ExceptionCodePrefix}UnexpectedException:001";
    public const string Success = $"{ExceptionCodePrefix}Success:001";
    
    public static class Mail
    {
        private const string Prefix = $"{ExceptionCodePrefix}:Mail:";
        public const string SettingNotFound = $"{Prefix}001";
    }
    
    public static class IdentityUser
    {
        private const string Prefix = $"{ExceptionCodePrefix}:IdentityUser:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    
    public static class Capsule
    {
        private const string Prefix = $"{ExceptionCodePrefix}:Capsule:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    
    public static class CapsuleItem
    {
        private const string Prefix = $"{ExceptionCodePrefix}:CapsuleItem:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
}