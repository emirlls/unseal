using Unseal.Entities.Capsules;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Capsules;

public class EfCapsuleMapFeatureRepository : EfBaseRepository<CapsuleMapFeature>, ICapsuleMapFeatureRepository
{
    public EfCapsuleMapFeatureRepository(
        IDbContextProvider<UnsealDbContext> dbContextProvider
    ) : base(dbContextProvider)
    {
    }
}