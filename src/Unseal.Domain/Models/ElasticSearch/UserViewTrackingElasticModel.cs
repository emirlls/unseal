using System;

namespace Unseal.Models.ElasticSearch;

public class UserViewTrackingElasticModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExternalId { get; set; }
    public Guid? UserViewTrackingTypeId { get; set; }
}