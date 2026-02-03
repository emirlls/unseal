using System;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Localization;
using Unseal.Models.Capsules;
using Unseal.Repositories.Capsules;

namespace Unseal.Managers.Capsules;

public class CapsuleManager : BaseDomainService<Capsule>, ICapsuleManager
{
    public CapsuleManager(
        ICapsuleRepository baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
        ) : 
        base(baseRepository,
            stringLocalizer,
            ExceptionCodes.Capsule.NotFound,
            ExceptionCodes.Capsule.AlreadyExists
        )
    {
    }

    public Capsule Create(CapsuleCreateModel capsuleCreateModel, Guid? creatorId)
    {
        var capsule = new Capsule(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            capsuleCreateModel.ReceiverId,
            capsuleCreateModel.CapsuleTypeId,
            creatorId,
            capsuleCreateModel.Name,
            capsuleCreateModel.RevealDate
        );
        return capsule;
    }
}