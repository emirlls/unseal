using Unseal.Workers.Workers.Capsules;
using Unseal.Workers.Workers.Users;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace Unseal.Workers;
[DependsOn(
    typeof(UnsealDomainModule),
    typeof(UnsealApplicationContractsModule),
    typeof(UnsealApplicationModule))]
public class UnsealWorkerModule : AbpModule
{
    public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<CapsuleRevealWorker>();
        await context.AddBackgroundWorkerAsync<UserLastActivityUpdateWorker>();
        await context.AddBackgroundWorkerAsync<UserViewTrackingWorker>();
    }
}