namespace Unseal.Constants;

public static class ExceptionCodes
{
    private const string ExceptionCodePrefix = "Unseal:ExceptionCodes:";
    public const string UnexpectedException = $"{ExceptionCodePrefix}UnexpectedException:001";
    public const string Success = $"{ExceptionCodePrefix}Success:001";
    
    public static class Mail
    {
        private const string Prefix = $"{ExceptionCodePrefix}Mail:";
        public const string SettingNotFound = $"{Prefix}001";
    }
    
    public static class IdentityUser
    {
        private const string Prefix = $"{ExceptionCodePrefix}IdentityUser:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
        public const string CannotLoginIfMailNotConfirmed = $"{Prefix}003";
        public const string MailInUser = $"{Prefix}004";
        public const string MailIsInvalid = $"{Prefix}005";
    }
    
    public static class Capsule
    {
        private const string Prefix = $"{ExceptionCodePrefix}Capsule:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    
    public static class CapsuleItem
    {
        private const string Prefix = $"{ExceptionCodePrefix}CapsuleItem:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    public static class CapsuleComment
    {
        private const string Prefix = $"{ExceptionCodePrefix}CapsuleComment:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    public static class CapsuleLike
    {
        private const string Prefix = $"{ExceptionCodePrefix}CapsuleLike:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    public static class CapsuleMapFeature
    {
        private const string Prefix = $"{ExceptionCodePrefix}CapsuleMapFeature:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    public static class NotificationTemplate
    {
        private const string Prefix = $"{ExceptionCodePrefix}NotificationTemplate:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    
    public static class NotificationEventType
    {
        private const string Prefix = $"{ExceptionCodePrefix}NotificationEventType:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    
    public static class File
    {
        private const string Prefix = $"{ExceptionCodePrefix}File:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
        public const string UploadError = $"{Prefix}003";
    }
    
    public static class UserProfile
    {
        private const string Prefix = $"{ExceptionCodePrefix}UserProfile:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    public static class UserFollower
    {
        private const string Prefix = $"{ExceptionCodePrefix}UserFollower:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
        public const string UserIsBanned = $"{Prefix}003";
    }
    public static class Group
    {
        private const string Prefix = $"{ExceptionCodePrefix}Group:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    public static class GroupMember
    {
        private const string Prefix = $"{ExceptionCodePrefix}GroupMember:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
        public const string UserNotAllowedToJoinGroup = $"{Prefix}003";
    }
    public static class UserInteraction
    {
        private const string Prefix = $"{ExceptionCodePrefix}UserInteraction:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    public static class ChatMessage
    {
        private const string Prefix = $"{ExceptionCodePrefix}ChatMessage:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    public static class ChatType
    {
        private const string Prefix = $"{ExceptionCodePrefix}ChatType:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
    public static class UserFollowStatus
    {
        private const string Prefix = $"{ExceptionCodePrefix}UserFollowStatus:";
        public const string NotFound = $"{Prefix}001";
        public const string AlreadyExists = $"{Prefix}002";
    }
}