using Unseal.Entities.Groups;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Groups;

public class EfGroupRepository : EfBaseRepository<Group>, IGroupRepository
{
    public EfGroupRepository(
        IDbContextProvider<UnsealDbContext> dbContextProvider) : 
        base(dbContextProvider)
    {
    }
}