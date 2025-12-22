using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Unseal;

[DependsOn(
    typeof(UnsealDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class UnsealApplicationContractsModule : AbpModule
{

}
