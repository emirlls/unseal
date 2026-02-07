using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;
using IdentityRole = Volo.Abp.Identity.IdentityRole;

namespace Unseal.Repositories.Auth;

public class EfCustomRoleRepository : EfBaseRepository<IdentityRole>, ICustomRoleRepository
{
    public EfCustomRoleRepository(IDbContextProvider<UnsealDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}