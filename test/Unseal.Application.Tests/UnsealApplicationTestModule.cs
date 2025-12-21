using Volo.Abp.Modularity;

namespace Unseal;

[DependsOn(
    typeof(UnsealApplicationModule),
    typeof(UnsealDomainTestModule)
    )]
public class UnsealApplicationTestModule : AbpModule
{

}
