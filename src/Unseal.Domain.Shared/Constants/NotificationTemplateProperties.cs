namespace Unseal.Constants;

public static class NotificationTemplateProperties
{
    public const string AppName = "Unseal";
    public static class RegisterTemplateParameters
    {
        public const string Name = "{{ Name }}";
        public const string Surname = "{{ Surname }}";
        public const string VerifyEmailUrl = "{{ VerifyEmailUrl }}";
    }
}