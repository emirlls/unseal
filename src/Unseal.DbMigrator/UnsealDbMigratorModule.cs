using Unseal.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace Unseal.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(UnsealEntityFrameworkCoreModule),
    typeof(UnsealApplicationContractsModule)
)]
public class UnsealDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundWorkerOptions>(options =>
        {
            options.IsEnabled = false;
        });

    }
}