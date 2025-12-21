using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.EntityFrameworkCore;

public class UnsealHttpApiHostMigrationsDbContext : AbpDbContext<UnsealHttpApiHostMigrationsDbContext>
{
    public UnsealHttpApiHostMigrationsDbContext(DbContextOptions<UnsealHttpApiHostMigrationsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureUnseal();
    }
}
