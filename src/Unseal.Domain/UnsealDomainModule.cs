using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Unseal;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(UnsealDomainSharedModule)
)]
public class UnsealDomainModule : AbpModule
{

}
