using Unseal.Entities.Messages;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Messages;

public class EfChatMessageRepository : EfBaseRepository<ChatMessage>, IChatMessageRepository
{
    public EfChatMessageRepository(
        IDbContextProvider<UnsealDbContext> dbContextProvider
        ) : base(dbContextProvider)
    {
    }
}