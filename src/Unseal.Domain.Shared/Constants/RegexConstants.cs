namespace Unseal.Constants;

public static class RegexConstants
{
    public const string MailRegexFormat = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public const string PasswordRegexFormat = @"^(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
}