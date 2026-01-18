using Unseal.Entities.Capsules;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Capsules;

public class EfCapsuleLikeRepository : EfBaseRepository<CapsuleLike>, ICapsuleLikeRepository
{
    public EfCapsuleLikeRepository(
        IDbContextProvider<UnsealDbContext> dbContextProvider
    ) : base(dbContextProvider)
    {
    }
}