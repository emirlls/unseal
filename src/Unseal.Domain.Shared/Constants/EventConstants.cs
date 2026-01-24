namespace Unseal.Constants;

public static class EventConstants
{
    public static class EventBus
    {
        public const string MailConfirmation = nameof(MailConfirmation);
        public const string UserDelete = nameof(UserDelete);
        public const string UserRegister = nameof(UserRegister);
        public const string CreateUserProfile = nameof(CreateUserProfile);
    }

    public static class ServerSentEvents
    {
        public static class CapsuleCreate
        {
            public const string GlobalFeedUpdates = "global_feed_updates";
            public const string Type = nameof(CapsuleCreate);
            public const string LastEventId = "Last-Event-ID";
        }
    }
}