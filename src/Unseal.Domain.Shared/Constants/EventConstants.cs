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
            public const string GlobalFeedUpdateChannel = "global_feed_update";
            public const string Type = nameof(CapsuleCreate);
        }
        public static class FollowRequestAccept
        {
            public const string FollowRequestAcceptChannel = "follow_request_accept";
            public const string Type = nameof(FollowRequestAccept);
        }
    }
}