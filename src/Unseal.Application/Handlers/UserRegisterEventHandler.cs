using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unseal.Constants;
using Unseal.Enums;
using Unseal.Etos;
using Unseal.Extensions;
using Unseal.Interfaces.Managers.Notifications;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Unseal.Handlers;

public class UserRegisterEventHandler : 
    IDistributedEventHandler<UserRegisterEto>, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public UserRegisterEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleEventAsync(UserRegisterEto eventData)
    {
        var configuration = _serviceProvider
            .GetRequiredService<IConfiguration>();
        
        var notificationEventTypeManager = _serviceProvider
            .GetRequiredService<INotificationEventTypeManager>();
        
        var notificationEventType =
            await notificationEventTypeManager.TryGetByAsync(x => 
                x.Code == (int)NotificationEventTypes.UserRegister,
                throwIfNull:true);
        
        var notificationTemplateManager = _serviceProvider
            .GetRequiredService<INotificationTemplateManager>();
        
        var notificationTemplate =
            await notificationTemplateManager.TryGetByAsync(
                x => 
                    x.NotificationEventTypeId.Equals(notificationEventType.Id) &&
                string.Equals(x.Culture,CultureInfo.CurrentCulture.Name),
                throwIfNull: true);
        
        var baseUrl = configuration["App:SelfUrl"]; 
        var verifyUrl = $"{baseUrl}/api/auth/confirm-email?userId={eventData.UserId}&token={WebUtility.UrlEncode(eventData.ConfirmationToken)}";
        
        var replacedTemplate = notificationTemplate.Content
            .Replace(NotificationTemplateProperties.UserRegisterTemplateParameters.Name, eventData.Name)
            .Replace(NotificationTemplateProperties.UserRegisterTemplateParameters.Surname, eventData.Surname)
            .Replace(NotificationTemplateProperties.UserRegisterTemplateParameters.VerifyEmailUrl, verifyUrl)
            .Replace(NotificationTemplateProperties.UserRegisterTemplateParameters.ApplicationName, AppConstants.AppName);
        
        await _serviceProvider
            .SendMailAsync(eventData.Email,
                notificationTemplate.Subject ?? AppConstants.AppName,
                replacedTemplate
            );
    }
}