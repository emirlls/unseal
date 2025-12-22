using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace Unseal.Repositories.Auth;

public class EfCustomIdentityUserRepository : EfBaseRepository<IdentityUser>, ICustomIdentityUserRepository
{
    public EfCustomIdentityUserRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : 
        base(dbContextProvider)
    {
    }
}