using System;
using Unseal.Constants;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;

namespace Unseal.Etos;

[EventName(EventConstants.EventBus.UserRegister)]
public class UserRegisterEto : EtoBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string ConfirmationToken { get; set; }
}