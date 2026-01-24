using System;
using Riok.Mapperly.Abstractions;
using Unseal.Dtos.Messages;
using Unseal.Models.Messages;
using Volo.Abp.DependencyInjection;

namespace Unseal.Profiles.Messages;

[Mapper]
public partial class ChatMessageMapper : ITransientDependency
{
    public partial ChatMessageCreateModel MapToModel(ChatMessageCreateDto dto, Guid senderId);
}