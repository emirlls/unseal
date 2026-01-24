using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unseal.Constants;
using Unseal.Workers.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Unseal.Workers.Workers.Users;

public class UserLastActivityUpdateWorker : AsyncPeriodicBackgroundWorkerBase
{
    public UserLastActivityUpdateWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory
    ) : base(
        timer,
        serviceScopeFactory
    )
    {
        Timer.Period = 10 * 60 * 1000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var configuration = ServiceProvider.GetRequiredService<IConfiguration>();
        var userLastActivityUpdateBackgroundJobSetting =
            bool.Parse(configuration[
                BackgroundJobSettingConstants.UserLastActivityUpdate.UserLastActivityUpdateBackgroundJob]!);
        if (!userLastActivityUpdateBackgroundJobSetting)
        {
            return;
        }

        var userLastActivityUpdateBackgroundJob =
            ServiceProvider.GetRequiredService<UserLastActivityUpdateBackgroundJob>();
        await userLastActivityUpdateBackgroundJob.UpdateLastActivityAsync(
            ServiceProvider,
            workerContext.CancellationToken
        );
    }
}