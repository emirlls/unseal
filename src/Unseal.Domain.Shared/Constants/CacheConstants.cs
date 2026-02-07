namespace Unseal.Constants;

public static class CacheConstants
{
    public const string RedisConfigurationKey = "Redis:Configuration";
    public const string CacheKeyPrefixKey = "Redis:CacheKeyPrefix";
    public const string PermissionGrantCacheItem = "pn:{0},pk:{1}*";
    
    public static class UserActivity
    {
        public const string IndexKeyPrefix = nameof(UserActivity);
        public const string RedisCacheKey = $"{IndexKeyPrefix}:"+"{0}";
        public const string MemoryCacheKey = $"{IndexKeyPrefix}:"+"{0}";
    }
}