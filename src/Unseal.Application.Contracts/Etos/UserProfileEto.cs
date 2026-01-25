using System;
using Unseal.Constants;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;

namespace Unseal.Etos;

[EventName(EventConstants.EventBus.CreateUserProfile)]
public class UserProfileEto : EtoBase
{
    public Guid UserId { get; set; }
}