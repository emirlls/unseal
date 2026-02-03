using System;
using Unseal.Attributes;
using Unseal.Filtering.Base;

namespace Unseal.Filtering.Groups;

public class GroupFilters: DynamicFilterRequest
{
    [FilterMapped("Name")]
    public string? Name { get; set; }

    [FilterMapped("Description")]
    public string? Description { get; set; }

    [FilterMapped("CreationTime")]
    public DateTime? CreationTime { get; set; }
}