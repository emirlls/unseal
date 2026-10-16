using System;
using System.Collections.Generic;
using Unseal.Constants;

namespace Unseal.Filtering.Base;

public class DynamicFilterRequest
{
    private int _skipCount;
    private int _maxResultCount = 10;

    public List<FilterItem> Filters { get; set; } = new();
    public string? Sorting { get; set; }

    public int SkipCount
    {
        get => _skipCount;
        set => _skipCount = Math.Max(0, value);
    }

    // Page size is capped so a single request cannot pull the whole table.
    public int MaxResultCount
    {
        get => _maxResultCount;
        set => _maxResultCount = Math.Clamp(value, 1, AppConstants.Filtering.MaxResultCount);
    }
}
