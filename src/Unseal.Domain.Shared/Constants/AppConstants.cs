namespace Unseal.Constants;

public static class AppConstants
{
    public const string AppName = "Unseal";
    public static class GenericResponse
    {
        public const string ContentType = "application/json";
        public const string Code = "code";
        public const string Message = "message";
        public const string Details = "details";
        public const string Error = "error";
    }

    public static class DataProtection
    {
        public const string Purpose = "Unseal.FileEncryption";
    }
}