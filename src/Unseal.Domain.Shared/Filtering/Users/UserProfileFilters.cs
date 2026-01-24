using System;
using Unseal.Attributes;
using Unseal.Filtering.Base;

namespace Unseal.Filtering.Users;

public class UserProfileFilters : DynamicFilterRequest
{
    [FilterMapped("UserId")]
    public Guid UserId { get; set; }
    
    [FilterMapped("CreationTime")]
    public DateTime? CreationTime { get; set; }
}