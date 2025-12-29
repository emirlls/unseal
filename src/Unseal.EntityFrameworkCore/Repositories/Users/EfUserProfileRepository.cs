using Unseal.Entities.Users;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Users;

public class EfUserProfileRepository : EfBaseRepository<UserProfile>, IUserProfileRepository
{
    public EfUserProfileRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}