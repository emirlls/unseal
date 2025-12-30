using System;
using Volo.Abp.Application.Dtos;

namespace Unseal.Filtering.Capsules;

public class CapsuleFilters : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? Name { get; set; }
    public DateTime? RevealDateMin { get; set; }
    public DateTime? RevealDateMax { get; set; }
}