using Unseal.Entities.Lookups;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Lookups;

public class EfCapsuleTypeRepository : EfBaseRepository<CapsuleType>, ICapsuleTypeRepository
{
    public EfCapsuleTypeRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : 
        base(dbContextProvider)
    {
    }
}