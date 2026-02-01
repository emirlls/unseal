using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unseal.Constants;
using Unseal.Workers.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Unseal.Workers.Workers.Users;

public class UserViewTrackingWorker : AsyncPeriodicBackgroundWorkerBase
{
    public UserViewTrackingWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory
        ) : base(timer, serviceScopeFactory)
    {
        Timer.Period = 24 * 60 * 60 * 1000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var configuration = ServiceProvider.GetRequiredService<IConfiguration>();

        var isEnabled = configuration.GetValue(BackgroundJobSettingConstants.UserViewTracking.UserViewTrackingBackgroundJob, false);
        if (!isEnabled) return;
        var userViewTrackingBackgroundJob =
            ServiceProvider.GetRequiredService<UserViewTrackingBackgroundJob>();
        await userViewTrackingBackgroundJob.CleanUserViewTrackingAsync(
            ServiceProvider,
            workerContext.CancellationToken
        );
    }
}