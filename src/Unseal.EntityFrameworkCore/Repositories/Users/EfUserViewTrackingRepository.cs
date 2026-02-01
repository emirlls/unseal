using Unseal.Entities.Users;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Users;

public class EfUserViewTrackingRepository : EfBaseRepository<UserViewTracking>, IUserViewTrackingRepository
{
    public EfUserViewTrackingRepository(IDbContextProvider<UnsealDbContext> dbContextProvider
    ) : base(dbContextProvider)
    {
    }
}