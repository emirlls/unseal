using Unseal.Entities.Lookups;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Lookups;

public class EfUserFollowStatusRepository : EfBaseRepository<UserFollowStatus>, IUserFollowStatusRepository
{
    public EfUserFollowStatusRepository(
        IDbContextProvider<UnsealDbContext> dbContextProvider
    ) : base(
        dbContextProvider
    )
    {
    }
}