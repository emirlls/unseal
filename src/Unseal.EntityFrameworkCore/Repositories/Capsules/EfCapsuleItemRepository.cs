using Unseal.Entities.Capsules;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Capsules;

public class EfCapsuleItemRepository : EfBaseRepository<CapsuleItem>, ICapsuleItemRepository
{
    public EfCapsuleItemRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : 
        base(dbContextProvider)
    {
    }
}