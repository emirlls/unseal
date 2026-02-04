using System.Collections.Generic;

namespace Unseal.Dtos.Dashboards;

public class DashboardDto
{
    public List<string> Labels { get; set; }
    public List<DashboardItemsDto> DashboardItems { get; set; }
}