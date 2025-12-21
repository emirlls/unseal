using Volo.Abp.Modularity;

namespace Unseal;

[DependsOn(
    typeof(UnsealDomainModule),
    typeof(UnsealTestBaseModule)
)]
public class UnsealDomainTestModule : AbpModule
{

}
