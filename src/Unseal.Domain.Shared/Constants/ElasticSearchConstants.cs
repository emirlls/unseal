namespace Unseal.Constants;

public static class ElasticSearchConstants
{
    public const string ElasticsearchOptions = nameof(ElasticsearchOptions);
    public const int ElasticPageSize = 999999999;
    public const string DefaultIndex = "default";
    public const string IdPropertyName = "Id";

    public static class User
    {
        public const string UserViewTrackingIndex = "user-view-tracking-index";
    }
}