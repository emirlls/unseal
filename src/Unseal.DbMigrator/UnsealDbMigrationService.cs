using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unseal.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Unseal.DbMigrator;

public class UnsealDbMigrationService : ITransientDependency
{
    private readonly IDataSeeder _dataSeeder;
    private readonly UnsealDbContext _dbContext;
    public ILogger<UnsealDbMigrationService> Logger { get; set; }

    public UnsealDbMigrationService(
        IDataSeeder dataSeeder,
        UnsealDbContext dbContext,
        ILogger<UnsealDbMigrationService> logger
    )
    {
        _dataSeeder = dataSeeder;
        _dbContext = dbContext;
        Logger = logger;
    }


    public async Task MigrateAsync()
    {
        Logger.LogInformation("Migration işlemi başlatılıyor...");

        await _dbContext.Database.MigrateAsync();

        await _dataSeeder.SeedAsync();
        Logger.LogInformation("Migration ve Seeding başarıyla tamamlandı!");
    }
}