using Unseal.Workers.Workers.Capsules;
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
    }
}