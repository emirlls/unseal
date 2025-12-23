using Microsoft.Extensions.DependencyInjection;
using Unseal.Settings.Custom;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Unseal;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(UnsealDomainSharedModule)
)]
public class UnsealDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient(typeof(ICustomSettingManager<>), typeof(CustomSettingManager<>));
    }
}
