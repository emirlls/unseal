using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Localization;
using Unseal.Repositories.Base;

namespace Unseal.Managers.Capsules;

public class CapsuleManager : BaseDomainService<Capsule>, ICapsuleManager
{
    public CapsuleManager(
        IBaseRepository<Capsule> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
        ) : 
        base(baseRepository,
            stringLocalizer,
            ExceptionCodes.Capsule.NotFound,
            ExceptionCodes.Capsule.AlreadyExists
        )
    {
    }
}