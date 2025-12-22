using Microsoft.EntityFrameworkCore;
using Unseal.Extensions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Authorizations;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.OpenIddict.Tokens;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Unseal.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ReplaceDbContext(typeof(IOpenIddictDbContext))]
[ReplaceDbContext(typeof(IPermissionManagementDbContext))]
[ReplaceDbContext(typeof(ISettingManagementDbContext))]
[ReplaceDbContext(typeof(IFeatureManagementDbContext))]
[ConnectionStringName(UnsealDbProperties.ConnectionStringName)]
public class UnsealDbContext : 
    AbpDbContext<UnsealDbContext>,
    IUnsealDbContext,
    IIdentityDbContext,
    ITenantManagementDbContext,
    IOpenIddictDbContext,
    IPermissionManagementDbContext,
    ISettingManagementDbContext, IFeatureManagementDbContext
{
    
    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    //Tenant
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }
    //Open Iddict
    public DbSet<OpenIddictApplication> Applications { get; set; }
    public DbSet<OpenIddictAuthorization> Authorizations { get; set; }
    public DbSet<OpenIddictScope> Scopes { get; set; }
    public DbSet<OpenIddictToken> Tokens { get; set; }
    //Permission
    public DbSet<PermissionGroupDefinitionRecord> PermissionGroups { get; set; }
    public DbSet<PermissionDefinitionRecord> Permissions { get; set; }
    public DbSet<PermissionGrant> PermissionGrants { get; set; }
    //Setting
    public DbSet<Setting> Settings { get; set; }
    public DbSet<SettingDefinitionRecord> SettingDefinitionRecords { get; set; }
    //Feature
    public DbSet<FeatureGroupDefinitionRecord> FeatureGroups { get; }
    public DbSet<FeatureDefinitionRecord> Features { get; }
    public DbSet<FeatureValue> FeatureValues { get; }

    public UnsealDbContext(DbContextOptions<UnsealDbContext> options)
        : base(options)
    {

    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.SetAbpTablePrefix();

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureFeatureManagement();
        builder.ConfigureUnseal();
        builder.ApplyConfigurationsFromAssembly(typeof(UnsealDbContext).Assembly);
        builder.ToSnakeCase();
        
    }
}
