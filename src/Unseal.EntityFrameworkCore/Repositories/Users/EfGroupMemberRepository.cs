using Unseal.Entities.Users;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Users;

public class EfGroupMemberRepository : EfBaseRepository<GroupMember>, IGroupMemberRepository
{
    public EfGroupMemberRepository(
        IDbContextProvider<UnsealDbContext> dbContextProvider) : 
        base(dbContextProvider)
    {
    }
}