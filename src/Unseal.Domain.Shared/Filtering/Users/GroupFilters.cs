using Volo.Abp.Application.Dtos;

namespace Unseal.Filtering.Users;

public class GroupFilters: PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? Name { get; set; }
}