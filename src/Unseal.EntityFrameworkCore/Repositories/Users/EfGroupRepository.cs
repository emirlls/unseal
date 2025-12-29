using Unseal.Entities.Users;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Users;

public class EfGroupRepository : EfBaseRepository<Group>, IGroupRepository
{
    public EfGroupRepository(
        IDbContextProvider<UnsealDbContext> dbContextProvider) : 
        base(dbContextProvider)
    {
    }
}