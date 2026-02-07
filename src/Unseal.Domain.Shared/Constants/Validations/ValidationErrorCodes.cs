namespace Unseal.Constants.Validations;

public static class ValidationErrorCodes
{
    public const string ValidationPrefix = "Validation:";

    #region Auth
    public static class Auth
        {
            public static class RegisterDto
            {
                private const string Prefix = $"{ValidationPrefix}{nameof(RegisterDto)}:"; 
                public const string EmailIsRequired = $"{Prefix}{nameof(EmailIsRequired)}";
                public const string InvalidEmailFormat = $"{Prefix}{nameof(InvalidEmailFormat)}";
                public const string PasswordIsRequired = $"{Prefix}{nameof(PasswordIsRequired)}";
                public const string InvalidPasswordFormat = $"{Prefix}{nameof(InvalidPasswordFormat)}";
                public const string ConfirmPasswordIsRequired = $"{Prefix}{nameof(ConfirmPasswordIsRequired)}";
                public const string FirstNameIsRequired = $"{Prefix}{nameof(FirstNameIsRequired)}";
                public const string LastNameIsRequired = $"{Prefix}{nameof(LastNameIsRequired)}";
                public const string PasswordsDontMatch = $"{Prefix}{nameof(PasswordsDontMatch)}";
            }
            
            public static class LoginDto
            {
                private const string Prefix = $"{ValidationPrefix}{nameof(LoginDto)}:"; 
                public const string EmailIsRequired = $"{Prefix}{nameof(EmailIsRequired)}";
                public const string PasswordIsRequired = $"{Prefix}{nameof(PasswordIsRequired)}";
            }
    
            public static class ChangePasswordDto
            {
                private const string Prefix = $"{ValidationPrefix}{nameof(ChangePasswordDto)}:"; 
                public const string OldPasswordIsRequired = $"{Prefix}{nameof(OldPasswordIsRequired)}";
                public const string NewPasswordIsRequired = $"{Prefix}{nameof(NewPasswordIsRequired)}";
                public const string InvalidPasswordFormat = $"{Prefix}{nameof(InvalidPasswordFormat)}";
            }
        }
    #endregion

    #region Capsules

    public static class Capsules
    {
        public static class CapsuleCreateDto
        {
            private const string Prefix = $"{ValidationPrefix}{nameof(CapsuleCreateDto)}:"; 
            public const string NameIsRequired = $"{Prefix}{nameof(NameIsRequired)}";
            public const string RevealDateIsRequired = $"{Prefix}{nameof(RevealDateIsRequired)}";
            public const string RevealDateMustBeInFuture = $"{Prefix}{nameof(RevealDateMustBeInFuture)}";
            public const string InvalidDateFormat = $"{Prefix}{nameof(InvalidDateFormat)}";
        }
    }

    #endregion
    
    #region Groups

    public static class Groups
    {
        public static class GroupCreateDto
        {
            private const string Prefix = $"{ValidationPrefix}{nameof(GroupCreateDto)}:"; 
            public const string UsersIsRequired = $"{Prefix}{nameof(UsersIsRequired)}";
            public const string NameIsRequired = $"{Prefix}{nameof(NameIsRequired)}";
        }
    }

    #endregion
    
    #region Messages

    public static class Messages
    {
        public static class ChatMessageCreateDto
        {
            private const string Prefix = $"{ValidationPrefix}{nameof(ChatMessageCreateDto)}:"; 
            public const string TargetIsRequired = $"{Prefix}{nameof(TargetIsRequired)}";
            public const string ChatTypeIsRequired = $"{Prefix}{nameof(ChatTypeIsRequired)}";
            public const string ContentIsRequired = $"{Prefix}{nameof(ContentIsRequired)}";
        }
    }

    #endregion
    
    #region Users

    public static class Users
    {
        public static class UserProfileUpdateDto
        {
            private const string Prefix = $"{ValidationPrefix}{nameof(UserProfileUpdateDto)}:"; 
            public const string IsLockedIsRequired = $"{Prefix}{nameof(IsLockedIsRequired)}";
            public const string AllowJoinGroupIsRequired = $"{Prefix}{nameof(AllowJoinGroupIsRequired)}";
        }

        public static class MarkAsViewedDto
        {
            private const string Prefix = $"{ValidationPrefix}{nameof(MarkAsViewedDto)}:"; 
            public const string TypeIsRequired = $"{Prefix}{nameof(TypeIsRequired)}";
            public const string ExternalIsRequired = $"{Prefix}{nameof(ExternalIsRequired)}";
        }
    }

    #endregion
}