using System.Net;
using Localization.Resources.AbpUi;
using Unseal.Constants;
using Unseal.Localization;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Unseal;

[DependsOn(
    typeof(UnsealApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class UnsealHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(UnsealHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<UnsealResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });

        Configure<AbpExceptionHttpStatusCodeOptions>(options =>
        {
            options.Map(ExceptionCodes.Filter.InvalidProperty, HttpStatusCode.BadRequest);
            options.Map(ExceptionCodes.Filter.InvalidStrategy, HttpStatusCode.BadRequest);
            options.Map(ExceptionCodes.Filter.InvalidValue, HttpStatusCode.BadRequest);
            options.Map(ExceptionCodes.Filter.InvalidSorting, HttpStatusCode.BadRequest);
        });
    }
}
