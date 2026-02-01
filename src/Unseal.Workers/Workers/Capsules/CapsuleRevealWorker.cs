using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unseal.Constants;
using Unseal.Workers.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Unseal.Workers.Workers.Capsules;

public class CapsuleRevealWorker : AsyncPeriodicBackgroundWorkerBase
{
    public CapsuleRevealWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
    {
        Timer.Period = 60 * 1000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var configuration = ServiceProvider.GetRequiredService<IConfiguration>();

        var isEnabled = configuration.GetValue(BackgroundJobSettingConstants.CapsuleReveal.CapsuleRevealBackgroundJob, false);
        if (!isEnabled) return;
        
        var capsuleRevealBackgroundJob = ServiceProvider.GetRequiredService<CapsuleRevealBackgroundJob>();
        await capsuleRevealBackgroundJob.RevealCapsulesAsync(
            ServiceProvider,
            workerContext.CancellationToken
        );
    }
}