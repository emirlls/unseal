namespace Unseal.Constants;

public static class SettingConstants
{
    public const string Prefix = "Unseal.Settings.";
    public static class MailSettingModel
    {
        public const string Name = nameof(MailSettingModel);
        public const string Gmail = nameof(Gmail);
        public const string Server = nameof(Server);
        public const string Port = nameof(Port);
        public const string Login = nameof(Login);
        public const string Key = nameof(Key);
    }
    public static class ElasticSearchSettingModel
    {
        public const string Name = nameof(ElasticSearchSettingModel);
        public const string Url = nameof(Url);
        public const string Username = nameof(Username);
        public const string Password = nameof(Password);
    }
}