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

    public static class Filtering
    {
        public const int MaxResultCount = 100;
    }

    public static class ChatMessage
    {
        public const int MaxContentLength = 2000;
        public const int MaxTargetCount = 50;
    }

    public static class CapsuleComment
    {
        public const int MaxCommentLength = 1000;
    }
}