using Unseal.Entities.Lookups;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Lookups;

public class EfChatTypeRepository : EfBaseRepository<ChatType>, IChatTypeRepository
{
    public EfChatTypeRepository(
        IDbContextProvider<UnsealDbContext> dbContextProvider
    ) : base(dbContextProvider)
    {
    }
}