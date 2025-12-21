using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Unseal;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(UnsealHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class UnsealConsoleApiClientModule : AbpModule
{

}
