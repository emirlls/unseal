using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Unseal.EntityFrameworkCore;

public class UnsealHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<UnsealDbContext>
{
    public UnsealDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        var builder = new DbContextOptionsBuilder<UnsealDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Default"));

        return new UnsealDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
