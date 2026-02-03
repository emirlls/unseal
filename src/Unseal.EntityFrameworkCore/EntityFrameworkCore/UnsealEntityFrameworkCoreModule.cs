using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Unseal.EntityFrameworkCore;

[DependsOn(
    typeof(UnsealDomainModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule)
)]
public class UnsealEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<UnsealDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<UnsealDbContext>(opts =>
            {
                opts.UseNpgsql(npgsqlOptions =>
                {
                    npgsqlOptions.UseNetTopologySuite();
                });
            });
        });
        context.Services.Replace(ServiceDescriptor.Transient<ISettingManagementDbContext, UnsealDbContext>());
        context.Services.Replace(ServiceDescriptor.Transient<IPermissionManagementDbContext, UnsealDbContext>());
        context.Services.Replace(ServiceDescriptor.Transient<IIdentityDbContext, UnsealDbContext>());
        context.Services.Replace(ServiceDescriptor.Transient<ITenantManagementDbContext, UnsealDbContext>());
        context.Services.Replace(ServiceDescriptor.Transient<IFeatureManagementDbContext, UnsealDbContext>());
        
        context.Services.AddTransient(typeof(IBaseRepository<>), typeof(EfBaseRepository<>));
    }
}
