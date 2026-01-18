using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Unseal.Constants;

namespace Unseal.EntityFrameworkCore;

public class UnsealHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<UnsealDbContext>
{
    public UnsealDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        var builder = new DbContextOptionsBuilder<UnsealDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Default"),
                opts => { opts.UseNetTopologySuite(); });

        return new UnsealDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../host/Unseal.HttpApi.Host"))
            .AddJsonFile(
                $"{MultiEnvironmentConstants.AspNetCoreEnvironmentAppSettingFile}{MultiEnvironmentConstants.AspNetCoreEnvironmentExtention}",
                optional: false)
            .AddJsonFile(
                $"{MultiEnvironmentConstants.AspNetCoreEnvironmentAppSettingFile}." +
                $"{Environment.GetEnvironmentVariable($"{MultiEnvironmentConstants.AspNetCoreEnvironment}")}" +
                $"{MultiEnvironmentConstants.AspNetCoreEnvironmentExtention}",
                true,
                true
            ).AddEnvironmentVariables();

        return builder.Build();
    }
}
