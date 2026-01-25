using System.Collections.Generic;
using Unseal.Entities.Messages;
using Unseal.Models.Messages;

namespace Unseal.Interfaces.Managers.Messages;

public interface IChatMessageManager : IBaseDomainService<ChatMessage>
{
    List<ChatMessage> Create(ChatMessageCreateModel chatMessageCreateModel);
}