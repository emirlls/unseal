using System;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Localization;
using Unseal.Repositories.Capsules;

namespace Unseal.Managers.Capsules;

public class CapsuleLikeManager : BaseDomainService<CapsuleLike>, ICapsuleLikeManager
{
    public CapsuleLikeManager(
        ICapsuleLikeRepository baseRepository,
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