using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Localization;
using Unseal.Settings.Custom;
using Unseal.Settings.Models;
using Volo.Abp;

namespace Unseal.Extensions;

public static class MailExtension
{
    public static async Task SendMailAsync(
        this IServiceProvider serviceProvider,
        string to,
        string subject,
        string body
    )
    {
        var stringLocalizer = serviceProvider.GetRequiredService<IStringLocalizer<UnsealResource>>();
        var customSettingManager = serviceProvider.GetRequiredService<ICustomSettingManager<MailSettingModel>>();
        var mailSettings = await customSettingManager.GetAsync<MailSettingModel>();

        if (mailSettings == null) throw new UserFriendlyException(stringLocalizer[ExceptionCodes.Mail.SettingNotFound]);

        using (var client = new SmtpClient(mailSettings.Server.Value!.ToString(),
                   int.Parse(mailSettings.Port.Value!.ToString()!)))
        {
            client.Credentials =
                new NetworkCredential(
                    mailSettings.Login.Value!.ToString(),
                    mailSettings.Key.Value!.ToString());
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(mailSettings.Login.Value.ToString()!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);
            await client.SendMailAsync(mailMessage);
        }
    }
}