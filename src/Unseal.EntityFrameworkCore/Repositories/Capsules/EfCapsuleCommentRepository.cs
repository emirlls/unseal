using Unseal.Entities.Capsules;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Capsules;

public class EfCapsuleCommentRepository : EfBaseRepository<CapsuleComment>, ICapsuleCommentRepository
{
    public EfCapsuleCommentRepository(
        IDbContextProvider<UnsealDbContext> dbContextProvider
    ) : base(dbContextProvider)
    {
    }
}