using System;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Dtos.Dashboards;
using Volo.Abp.Application.Services;

namespace Unseal.Services.Dashboards;

public interface IDashboardAppService : IApplicationService
{
    Task<DashboardDto> GetUsageByTimeAsync(
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default
    );

    Task<DashboardDto> GetCreationCapsuleByTimeAsync(
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default
    );
}