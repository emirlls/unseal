using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Application;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.RabbitMq;

namespace Unseal;

[DependsOn(
    typeof(UnsealDomainModule),
    typeof(UnsealApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpEventBusModule),
    typeof(AbpEventBusRabbitMqModule)
    )]
public class UnsealApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<UnsealApplicationModule>();
    }
}
