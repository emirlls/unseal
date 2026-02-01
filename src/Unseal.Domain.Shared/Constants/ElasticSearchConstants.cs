namespace Unseal.Constants;

public static class ElasticSearchConstants
{
    public const string ElasticsearchOptions = nameof(ElasticsearchOptions);
    public const int ElasticPageSize = 999999999;
    public const string DefaultIndex = "default";
    public const string IdPropertyName = "Id";

    public static class User
    {
        public const string UserSearchIndex = "user-search-index";
        public const string UserViewTrackingIndex = "user-view-tracking-index";
    }

    public static class Queries
    {
        public const string NewId = "newId";
       public const string AddIdScript = "if (!ctx._source.blockedByUserIds.contains(params.newId)) { ctx._source.blockedByUserIds.add(params.newId) }";
    }
}