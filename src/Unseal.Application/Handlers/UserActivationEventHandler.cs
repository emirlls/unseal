using System;
using System.Globalization;
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

public class UserActivationEventHandler : IDistributedEventHandler<UserActivationEto>, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public UserActivationEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleEventAsync(UserActivationEto eventData)
    {
        var notificationEventTypeManager = _serviceProvider
            .GetRequiredService<INotificationEventTypeManager>();

        var notificationEventType =
            await notificationEventTypeManager.TryGetByAsync(x =>
                    x.Code == (int)NotificationEventTypes.UserActivation,
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
            $"{baseUrl}/api/auth/{ApiConstants.Auth.ConfirmActivationMail}?userId={eventData.UserId}";

        var replacedTemplate = notificationTemplate.Content
            .Replace(NotificationTemplateProperties.ActivationMailTemplateParameters.Name, eventData.Name)
            .Replace(NotificationTemplateProperties.ActivationMailTemplateParameters.Surname, eventData.Surname)
            .Replace(NotificationTemplateProperties.ActivationMailTemplateParameters.VerifyEmailUrl, verifyUrl)
            .Replace(NotificationTemplateProperties.ActivationMailTemplateParameters.ApplicationName,
                AppConstants.AppName);

        await _serviceProvider
            .SendMailAsync(eventData.Email,
                notificationTemplate.Subject ?? AppConstants.AppName,
                replacedTemplate
            );
    }
}