using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Unseal.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenIddict.Validation.AspNetCore;
using Unseal.ActionFilters;
using Unseal.Constants;
using Unseal.Middlewares;
using Unseal.Models.ElasticSearch;
using Unseal.Workers;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.VirtualFileSystem;

namespace Unseal;

[DependsOn(
    typeof(UnsealApplicationModule),
    typeof(UnsealEntityFrameworkCoreModule),
    typeof(UnsealHttpApiModule),
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictAspNetCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule),
    typeof(UnsealWorkerModule)
)]
public class UnsealHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        Configure<AbpDistributedEventBusOptions>(options => { });
        Configure<AbpRabbitMqEventBusOptions>(options =>
        {
            options.ClientName = configuration["DistributedEventBus:ClientName"]!;
            options.ExchangeName = configuration["DistributedEventBus:ExchangeName"]!;
            ;
        });
        context.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                var endpoint = httpContext.GetEndpoint();
                var routePattern = endpoint?.Metadata.GetMetadata<RouteEndpoint>()?.RoutePattern.RawText
                                   ?? httpContext.Request.Path.ToString();
                if (routePattern.Contains(ApiConstants.Message.Endpoint))
                {
                    return RateLimitPartition.GetNoLimiter("messages-bypass");
                }

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: $"{ip}_{routePattern}",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
        context.Services.AddTransient<IDbConnection>(sp =>
        {
            var connectionString = configuration.GetConnectionString("Default");
            return new NpgsqlConnection(connectionString);
        });
        var redisConfiguration = configuration[CacheConstants.RedisConfigurationKey];
        context.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConfiguration!));
        context.Services.AddSingleton<ElasticsearchClient>(provider =>
        {
            
            var options = configuration.GetSection(nameof(SettingConstants.ElasticSearchSettingModel)).Get<ElasticSearchOptions>();
            var settings = new ElasticsearchClientSettings(new Uri(options.Url))
                .Authentication(new BasicAuthentication(options.Username, options.Password));
                // To bypass ssl on development.
                //.ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            return new ElasticsearchClient(settings);
        });
        
        Configure<MvcOptions>(options => { options.Filters.AddService<LastActivityActionFilter>(); });
        Configure<AbpMultiTenancyOptions>(options => { options.IsEnabled = MultiTenancyConsts.IsEnabled; });

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<UnsealDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        string.Format("..{0}..{0}src{0}Unseal.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<UnsealDomainModule>(Path.Combine(
                    hostingEnvironment.ContentRootPath,
                    string.Format("..{0}..{0}src{0}Unseal.Domain", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<UnsealApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        string.Format("..{0}..{0}src{0}Unseal.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<UnsealApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        string.Format("..{0}..{0}src{0}Unseal.Application", Path.DirectorySeparatorChar)));
            });
        }

        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"]!,
            new Dictionary<string, string>
            {
                { "Unseal", "Unseal API" }
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Unseal API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => ConfigureSwaggerNotVisibleApis(description));
                options.CustomSchemaIds(type => type.FullName);
                var baseDirectory = AppContext.BaseDirectory;
                var xmlFiles = Directory.GetFiles(baseDirectory, "*.xml");

                foreach (var xmlFile in xmlFiles)
                {
                    options.IncludeXmlComments(xmlFile);
                }
            });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
        });
        Configure<OpenIddictServerBuilder>(builder =>
        {
            builder.AddDevelopmentEncryptionCertificate();
            builder.AddDevelopmentSigningCertificate();
            builder.SetIssuer(new Uri(configuration["AuthServer:Authority"]));
        });

        context.Services.AddOpenIddict()
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        context.Services.Configure<IdentityOptions>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultProvider;
        });
        context.Services.AddAbpIdentity();
        context.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = httpContext =>
                {
                    var accessToken = httpContext.Request.Query["access_token"];

                    var path = httpContext.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/signalr-hubs")))
                    {
                        httpContext.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });
        context.Services.AddSignalR(options => { options.ClientTimeoutInterval = TimeSpan.FromMinutes(5); });
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "Unseal:"; });

        var dataProtectionBuilder = context.Services
            .AddDataProtection()
            .SetApplicationName(AppConstants.AppName);
        var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
        dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private static bool ConfigureSwaggerNotVisibleApis(ApiDescription apiDescription)
    {
        return !apiDescription.RelativePath!.StartsWith("api/abp/");
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCorrelationId();
        app.MapAbpStaticAssets();
        app.UseRouting();
        app.UseRateLimiter();
        app.UseCors();
        app.UseAuthentication();
        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseMiddleware<CustomTenantMiddleware>();

        app.UseAbpRequestLocalization();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support APP API");

            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes(AppConstants.AppName);
        });
        app.UseMiddleware<GenericResponseMiddleware>();

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}