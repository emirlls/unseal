using System;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;

namespace Unseal.Etos;

[EventName("UserRegister")]
public class UserRegisterEto : EtoBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string ConfirmationToken { get; set; }
}