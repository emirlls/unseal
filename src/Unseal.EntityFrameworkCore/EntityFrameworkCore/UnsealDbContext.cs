using Microsoft.EntityFrameworkCore;
using Unseal.Extensions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
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
[ConnectionStringName(UnsealDbProperties.ConnectionStringName)]
public class UnsealDbContext : 
    AbpDbContext<UnsealDbContext>,
    IUnsealDbContext,
    IIdentityDbContext,
    ITenantManagementDbContext,
    IOpenIddictDbContext,
    IPermissionManagementDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */
    
    //Identity
    public DbSet<IdentityUser> Users { get; }
    public DbSet<IdentityRole> Roles { get; }
    public DbSet<IdentityClaimType> ClaimTypes { get; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; }
    public DbSet<IdentityLinkUser> LinkUsers { get; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; }
    public DbSet<IdentitySession> Sessions { get; }
    //Tenant
    public DbSet<Tenant> Tenants { get; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; }
    //Open Iddict
    public DbSet<OpenIddictApplication> Applications { get; }
    public DbSet<OpenIddictAuthorization> Authorizations { get; }
    public DbSet<OpenIddictScope> Scopes { get; }
    public DbSet<OpenIddictToken> Tokens { get; }
    //Permission
    public DbSet<PermissionGroupDefinitionRecord> PermissionGroups { get; }
    public DbSet<PermissionDefinitionRecord> Permissions { get; }
    public DbSet<PermissionGrant> PermissionGrants { get; }
    public UnsealDbContext(DbContextOptions<UnsealDbContext> options)
        : base(options)
    {

    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.SetAbpTablePrefix();
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(UnsealDbContext).Assembly);

        builder.ConfigureUnseal();
        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        
        builder.ToSnakeCase();
        
    }
}
