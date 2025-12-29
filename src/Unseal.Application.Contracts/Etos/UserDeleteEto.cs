using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;

namespace Unseal.Etos;

[EventName("UserDelete")]
public class UserDeleteEto : EtoBase
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; } 
}