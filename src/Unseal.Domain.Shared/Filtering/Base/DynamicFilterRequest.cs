using System.Collections.Generic;

namespace Unseal.Filtering.Base;

public class DynamicFilterRequest
{
    public List<FilterItem> Filters { get; set; } = new();
    public string? Sorting { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 10;
}
