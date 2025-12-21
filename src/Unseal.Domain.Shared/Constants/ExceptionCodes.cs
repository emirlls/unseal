namespace Unseal.Constants;

public static class ExceptionCodes
{
    private const string ExceptionCodePrefix = "Unseal:ExceptionCodes:";
    public const string UnexpectedException = $"{ExceptionCodePrefix}UnexpectedException:001";
    public const string Success = $"{ExceptionCodePrefix}Success:001";
    
    public static class IdentityUser
    {
        private const string Prefix = $"{ExceptionCodePrefix}:IdentityUser:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
}