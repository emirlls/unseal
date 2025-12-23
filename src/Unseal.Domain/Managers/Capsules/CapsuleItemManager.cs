using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Localization;
using Unseal.Repositories.Base;

namespace Unseal.Managers.Capsules;

public class CapsuleItemManager : BaseDomainService<CapsuleItem>, ICapsuleItemManager
{
    public CapsuleItemManager(
        IBaseRepository<CapsuleItem> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
        )
        : base(
            baseRepository,
            stringLocalizer,
            ExceptionCodes.CapsuleItem.NotFound,
            ExceptionCodes.CapsuleItem.AlreadyExists
        )
    {
    }
}