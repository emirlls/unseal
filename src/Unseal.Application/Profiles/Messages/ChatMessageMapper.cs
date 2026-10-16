using System;
using Riok.Mapperly.Abstractions;
using Unseal.Dtos.Messages;
using Unseal.Models.Messages;
using Volo.Abp.DependencyInjection;

namespace Unseal.Profiles.Messages;

[Mapper]
public partial class ChatMessageMapper : ITransientDependency
{
    public ChatMessageCreateModel MapToModel(ChatMessageCreateDto dto, Guid senderId)
    {
        return new ChatMessageCreateModel
        {
            SenderId = senderId,
            TargetIds = dto.TargetIds,
            ChatTypeId = dto.ChatTypeId,
            Content = dto.Content
        };
    }
}