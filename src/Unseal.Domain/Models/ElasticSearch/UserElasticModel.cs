using System;
using System.Collections.Generic;

namespace Unseal.Models.ElasticSearch;

public class UserElasticModel
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public List<Guid>? BlockedByUserIds { get; set; } //Users that block current user.
}