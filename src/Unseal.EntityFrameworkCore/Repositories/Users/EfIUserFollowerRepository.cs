using Unseal.Entities.Users;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Users;

public class EfIUserFollowerRepository : EfBaseRepository<UserFollower>, IUserFollowerRepository
{
    public EfIUserFollowerRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}