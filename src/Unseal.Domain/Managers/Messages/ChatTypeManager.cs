using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Lookups;
using Unseal.Interfaces.Managers.Messages;
using Unseal.Localization;
using Unseal.Repositories.Lookups;

namespace Unseal.Managers.Messages;

public class ChatTypeManager : BaseDomainService<ChatType>,IChatTypeManager
{
    public ChatTypeManager(
        IChatTypeRepository baseRepository, 
        IStringLocalizer<UnsealResource> stringLocalizer
        ) : base(
        baseRepository, 
        stringLocalizer,
        ExceptionCodes.ChatType.NotFound,
        ExceptionCodes.ChatType.AlreadyExists
    )
    {
    }
}