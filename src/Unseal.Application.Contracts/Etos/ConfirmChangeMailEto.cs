using System;
using Unseal.Constants;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;

namespace Unseal.Etos;

[EventName(EventConstants.EventBus.MailConfirmation)]
public class ConfirmChangeMailEto : EtoBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string NewMailAddress { get; set; }
    public string Token { get; set; }
}