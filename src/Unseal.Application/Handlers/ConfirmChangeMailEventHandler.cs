using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Unseal.Constants;
using Unseal.Enums;
using Unseal.Etos;
using Unseal.Extensions;
using Unseal.Interfaces.Managers.Notifications;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Unseal.Handlers;

public class ConfirmChangeMailEventHandler : IDistributedEventHandler<ConfirmChangeMailEto>, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public ConfirmChangeMailEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleEventAsync(ConfirmChangeMailEto eventData)
    {
        var notificationEventTypeManager = _serviceProvider
            .GetRequiredService<INotificationEventTypeManager>();

        var notificationEventType =
            await notificationEventTypeManager.TryGetByAsync(x =>
                    x.Code == (int)NotificationEventTypes.ConfirmChangeMail,
                throwIfNull: true);

        var notificationTemplateManager = _serviceProvider
            .GetRequiredService<INotificationTemplateManager>();

        var notificationTemplate =
            await notificationTemplateManager.TryGetByAsync(
                x =>
                    x.NotificationEventTypeId.Equals(notificationEventType.Id) &&
                    string.Equals(x.Culture, CultureInfo.CurrentCulture.Name),
                throwIfNull: true);

        var baseUrl = await _serviceProvider.GetSelfUrlAsync();
        var verifyUrl =
            $"{baseUrl}/api/auth/{ApiConstants.Auth.ConfirmChangeMail}?userId={eventData.UserId}&newMailAddress={eventData.NewMailAddress}&token={WebUtility.UrlEncode(eventData.Token)}";

        var replacedTemplate = notificationTemplate.Content
            .Replace(NotificationTemplateProperties.ChangeMailTemplateParameters.Name, eventData.Name)
            .Replace(NotificationTemplateProperties.ChangeMailTemplateParameters.Surname, eventData.Surname)
            .Replace(NotificationTemplateProperties.ChangeMailTemplateParameters.VerifyEmailUrl, verifyUrl)
            .Replace(NotificationTemplateProperties.ChangeMailTemplateParameters.ApplicationName,
                AppConstants.AppName);

        await _serviceProvider
            .SendMailAsync(eventData.NewMailAddress,
                notificationTemplate.Subject ?? AppConstants.AppName,
                replacedTemplate
            );
    }
}