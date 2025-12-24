using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Notifications;
using Unseal.Interfaces.Managers.Notifications;
using Unseal.Localization;
using Unseal.Repositories.Base;

namespace Unseal.Managers.Notifications;

public class NotificationTemplateManager : BaseDomainService<NotificationTemplate>, INotificationTemplateManager
{
    public NotificationTemplateManager(
        IBaseRepository<NotificationTemplate> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
        ) : 
        base(
            baseRepository,
            stringLocalizer,
            ExceptionCodes.NotificationTemplate.NotFound,
            ExceptionCodes.NotificationTemplate.AlreadyExists
        )
    {
    }
}