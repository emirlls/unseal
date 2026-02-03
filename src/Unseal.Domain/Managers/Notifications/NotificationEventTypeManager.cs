using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Notifications;
using Unseal.Interfaces.Managers.Notifications;
using Unseal.Localization;
using Unseal.Repositories.Notifications;

namespace Unseal.Managers.Notifications;

public class NotificationEventTypeManager : BaseDomainService<NotificationEventType>, INotificationEventTypeManager
{
    public NotificationEventTypeManager(
        INotificationEventTypeRepository baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
    ) : base(baseRepository,
        stringLocalizer, 
        ExceptionCodes.NotificationEventType.NotFound,
        ExceptionCodes.NotificationEventType.AlreadyExists
    )
    {
    }
}