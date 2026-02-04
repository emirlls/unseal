using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Dtos.Dashboards;
using Unseal.Localization;
using Unseal.Repositories.Capsules;
using Unseal.Repositories.Users;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace Unseal.Services.Dashboards;

public class DashboardAppService : ApplicationService, IDashboardAppService
{
    private ICapsuleRepository CapsuleRepository =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleRepository>();

    private IUserProfileRepository UserProfileRepository =>
        LazyServiceProvider.LazyGetRequiredService<IUserProfileRepository>();

    private IStringLocalizer<UnsealResource> StringLocalizer =>
        LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<UnsealResource>>();
    
    private readonly IDataFilter<IMultiTenant> _dataFilter;

    public DashboardAppService(IDataFilter<IMultiTenant> dataFilter)
    {
        _dataFilter = dataFilter;
    }

    public async Task<DashboardDto> GetUsageByTimeAsync(
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default
    )
    {
        using (_dataFilter.Disable())
        {
            var userLastActivitiesByDate = await UserProfileRepository
                .GetLastActivityByDateAsync(
                    startDate,
                    endDate,
                    cancellationToken
                );
            var days = userLastActivitiesByDate.Select(x => x.Day).ToList();
            var counts = userLastActivitiesByDate.Select(x => x.Count).ToList();
            var response = new DashboardDto
            {
                Labels = days,
                DashboardItems = new List<DashboardItemsDto>()
                {
                    new DashboardItemsDto
                    {
                        Label = StringLocalizer[DashboardConstants.UsageByTime],
                        Data = new List<object>() { counts }
                    }
                }
            };
            return response;
        }
    }

    public async Task<DashboardDto> GetCreationCapsuleByTimeAsync(
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default
    )
    {
        using (_dataFilter.Disable())
        {
            var capsuleByDateModels = await CapsuleRepository
                .GetCreationCapsuleByDateAsync(
                    startDate,
                    endDate,
                    cancellationToken
                );
        
            var days = capsuleByDateModels.Select(x => x.Day).ToList();
            var counts = capsuleByDateModels.Select(x => x.Count).ToList();
            var response = new DashboardDto
            {
                Labels = days,
                DashboardItems = new List<DashboardItemsDto>()
                {
                    new DashboardItemsDto
                    {
                        Label = StringLocalizer[DashboardConstants.CreationCapsuleByTime],
                        Data = new List<object>() { counts }
                    }
                }
            };
            return response;
        }
    }
}