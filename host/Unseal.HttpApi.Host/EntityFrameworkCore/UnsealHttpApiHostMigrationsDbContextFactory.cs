using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Unseal.EntityFrameworkCore;

public class UnsealHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<UnsealHttpApiHostMigrationsDbContext>
{
    public UnsealHttpApiHostMigrationsDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<UnsealHttpApiHostMigrationsDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Unseal"));

        return new UnsealHttpApiHostMigrationsDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
