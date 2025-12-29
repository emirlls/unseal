using Unseal.Entities.Notifications;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Notifications;

public class EfNotificationTemplateRepository : EfBaseRepository<NotificationTemplate>, INotificationTemplateRepository
{
    public EfNotificationTemplateRepository(
        IDbContextProvider<UnsealDbContext> dbContextProvider) : 
        base(dbContextProvider)
    {
    }
}