using Unseal.Entities.Notifications;
using Unseal.EntityFrameworkCore;
using Unseal.Repositories.Base;
using Volo.Abp.EntityFrameworkCore;

namespace Unseal.Repositories.Notifications;

public class EfNotificationEventTypeRepository : EfBaseRepository<NotificationEventType>, INotificationEventTypeRepository
{
    public EfNotificationEventTypeRepository(
        IDbContextProvider<UnsealDbContext> dbContextProvider) : 
        base(dbContextProvider)
    {
    }
}