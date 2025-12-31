using Unseal.Entities.Users;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Users;

public class EfUserInteractionRepository : EfBaseRepository<UserInteraction>,IUserInteractionRepository
{
    public EfUserInteractionRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}