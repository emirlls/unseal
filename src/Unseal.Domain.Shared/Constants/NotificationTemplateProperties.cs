namespace Unseal.Constants;

public static class NotificationTemplateProperties
{
    public const string AppName = "Unseal";
    public static class UserRegisterTemplateParameters
    {
        public const string ApplicationName = "{{ApplicationName}}";
        public const string Name = "{{Name}}";
        public const string Surname = "{{Surname}}";
        public const string VerifyEmailUrl = "{{VerifyEmailUrl}}";
    }
    public static class UserDeleteTemplateParameters
    {
        public const string ApplicationName = "{{ApplicationName}}";
        public const string Name = "{{Name}}";
        public const string Surname = "{{Surname}}";
    }
}