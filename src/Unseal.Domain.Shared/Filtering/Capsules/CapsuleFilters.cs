using System;
using Unseal.Attributes;
using Unseal.Constants;
using Unseal.Filtering.Base;

namespace Unseal.Filtering.Capsules;

public class CapsuleFilters : DynamicFilterRequest
{
    [FilterMapped("Name")] 
    internal string? Name { get; set; }
    
    [FilterMapped("IsOpened")] 
    internal bool? IsOpened { get; set; }
    
    [FilterMapped("RevealDate")] 
    internal DateTime? RevealDate { get; set; }

    public static string GetName() => nameof(Name);
    public static string GetIsOpened() => nameof(IsOpened);
    public static string GetRevealDate() => nameof(RevealDate);

    public void SetIsOpened(bool value, string strategy = FilterOperators.Equals)
    {
        Filters.Add(new FilterItem 
        { 
            Prop = nameof(IsOpened), 
            Strategy = strategy, 
            Value = value.ToString().ToLower()
        });
    }
}