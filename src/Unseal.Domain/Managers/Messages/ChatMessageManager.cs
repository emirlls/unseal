using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Messages;
using Unseal.Interfaces.Managers.Messages;
using Unseal.Localization;
using Unseal.Models.Messages;
using Unseal.Repositories.Messages;

namespace Unseal.Managers.Messages;

public class ChatMessageManager : BaseDomainService<ChatMessage>, IChatMessageManager
{
    public ChatMessageManager(
        IChatMessageRepository baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
        ) : base(
        baseRepository, 
        stringLocalizer,
        ExceptionCodes.ChatMessage.NotFound,
        ExceptionCodes.ChatMessage.AlreadyExists
    )
    {
    }

    public List<ChatMessage> Create(ChatMessageCreateModel chatMessageCreateModel)
    {
        var chatMessages = new List<ChatMessage>();
        foreach (var targetId in chatMessageCreateModel.TargetIds)
        {
            chatMessages.Add(new ChatMessage(GuidGenerator.Create(),
                chatMessageCreateModel.SenderId,
                targetId,
                chatMessageCreateModel.ChatTypeId,
                chatMessageCreateModel.Content,
                DateTime.Now));
        }

        return chatMessages;
    }
}