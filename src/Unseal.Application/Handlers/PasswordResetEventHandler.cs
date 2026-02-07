using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Unseal.Constants;
using Unseal.Enums;
using Unseal.Etos;
using Unseal.Extensions;
using Unseal.Interfaces.Managers.Notifications;
using Volo.Abp.EventBus.Distributed;

namespace Unseal.Handlers;

public class PasswordResetEventHandler : IDistributedEventHandler<PasswordResetEto>
{
    private readonly IServiceProvider _serviceProvider;

    public PasswordResetEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleEventAsync(PasswordResetEto eventData)
    {
        var notificationEventTypeManager = _serviceProvider
            .GetRequiredService<INotificationEventTypeManager>();

        var notificationEventType =
            await notificationEventTypeManager.TryGetByAsync(x =>
                    x.Code == (int)NotificationEventTypes.PasswordReset,
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
            $"{baseUrl}/api/auth/{ApiConstants.Auth.ConfirmPasswordReset}?userId={eventData.UserId}&token={eventData.ConfirmationToken}";

        var replacedTemplate = notificationTemplate.Content
            .Replace(NotificationTemplateProperties.PasswordResetMailTemplateParameters.Name, eventData.Name)
            .Replace(NotificationTemplateProperties.PasswordResetMailTemplateParameters.Surname, eventData.Surname)
            .Replace(NotificationTemplateProperties.PasswordResetMailTemplateParameters.VerifyEmailUrl, verifyUrl)
            .Replace(NotificationTemplateProperties.PasswordResetMailTemplateParameters.ApplicationName,
                AppConstants.AppName);

        await _serviceProvider
            .SendMailAsync(eventData.Email,
                notificationTemplate.Subject ?? AppConstants.AppName,
                replacedTemplate
            );
    }
}