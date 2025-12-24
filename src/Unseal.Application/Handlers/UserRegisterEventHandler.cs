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
                x.Code == (int)NotificationEventTypes.Register,
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
        var verifyUrl = $"{baseUrl}/confirm-email?userId={eventData.UserId}&token={WebUtility.UrlEncode(eventData.ConfirmationToken)}";
        
        var replacedTemplate = notificationTemplate.Content
            .Replace(NotificationTemplateProperties.RegisterTemplateParameters.Name, eventData.Name)
            .Replace(NotificationTemplateProperties.RegisterTemplateParameters.Surname, eventData.Surname)
            .Replace(NotificationTemplateProperties.RegisterTemplateParameters.VerifyEmailUrl, verifyUrl);
        
        await _serviceProvider
            .SendMailAsync(eventData.Email,
                notificationTemplate.Subject ?? NotificationTemplateProperties.AppName,
                replacedTemplate
            );
    }
}