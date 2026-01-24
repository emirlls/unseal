using System;
using Unseal.Attributes;
using Unseal.Filtering.Base;

namespace Unseal.Filtering.Users;

public class UserInteractionFilters : DynamicFilterRequest
{
    [FilterMapped("SourceUserId")]
    public Guid SourceUserId { get; set; }
    
    [FilterMapped("TargetUserId")]
    public Guid TargetUserId { get; set; }
}