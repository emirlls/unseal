using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Unseal;

[DependsOn(
    typeof(UnsealApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class UnsealHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(UnsealApplicationContractsModule).Assembly,
            UnsealRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<UnsealHttpApiClientModule>();
        });

    }
}
