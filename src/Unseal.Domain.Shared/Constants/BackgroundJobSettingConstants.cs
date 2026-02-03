namespace Unseal.Constants;

public static class BackgroundJobSettingConstants
{
    private const string Prefix = "BackgroundJobSettings";
    
    public static class CapsuleReveal
    { 
        public const string CapsuleRevealBackgroundJob = $"{Prefix}:{nameof(CapsuleRevealBackgroundJob)}";
    }
    public static class UserLastActivityUpdate
    { 
        public const string UserLastActivityUpdateBackgroundJob = $"{Prefix}:{nameof(UserLastActivityUpdateBackgroundJob)}";
    }
    public static class UserViewTracking
    { 
        public const string UserViewTrackingBackgroundJob = $"{Prefix}:{nameof(UserViewTrackingBackgroundJob)}";
    }
}