using System.Collections.Generic;

namespace Unseal.Dtos.Dashboards;

public class DashboardItemsDto
{
    public string? Label { get; set; }
    public List<object> Data { get; set; }
}