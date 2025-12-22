using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.EntityFrameworkCore;

[ConnectionStringName(UnsealDbProperties.ConnectionStringName)]
public interface IUnsealDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
