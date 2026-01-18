using System;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Localization;
using Unseal.Repositories.Base;

namespace Unseal.Managers.Capsules;

public class CapsuleLikeManager : BaseDomainService<CapsuleLike>, ICapsuleLikeManager
{
    public CapsuleLikeManager(
        IBaseRepository<CapsuleLike> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
    ) : base(
        baseRepository,
        stringLocalizer,
        ExceptionCodes.CapsuleLike.NotFound,
        ExceptionCodes.CapsuleLike.AlreadyExists
    )
    {
    }

    public CapsuleLike Create(Guid capsuleId, Guid? currentUserId)
    {
        var capsuleLike = new CapsuleLike(
            GuidGenerator.Create(),
            (Guid)currentUserId,
            capsuleId, 
            DateTime.Now
        );
        return capsuleLike;
    }
}