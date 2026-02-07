using System;
using Unseal.Attributes;
using Unseal.Constants;
using Unseal.Filtering.Base;

namespace Unseal.Filtering.Users;

public class UserInteractionFilters : DynamicFilterRequest
{
    [FilterMapped("SourceUserId")]
    internal Guid SourceUserId { get; set; }
    
    [FilterMapped("TargetUserId")]
    internal Guid TargetUserId { get; set; }
    
    public void SetSourceUserId(Guid value, string strategy = FilterOperators.Equals)
    {
        Filters.Add(new FilterItem 
        { 
            Prop = nameof(SourceUserId), 
            Strategy = strategy, 
            Value = value.ToString().ToLower()
        });
    }
    public void SetTargetUserId(Guid value, string strategy = FilterOperators.Equals)
    {
        Filters.Add(new FilterItem 
        { 
            Prop = nameof(TargetUserId), 
            Strategy = strategy, 
            Value = value.ToString().ToLower()
        });
    }
}