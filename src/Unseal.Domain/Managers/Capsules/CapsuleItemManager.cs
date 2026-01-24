using System;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Localization;
using Unseal.Models.Capsules;
using Unseal.Repositories.Capsules;

namespace Unseal.Managers.Capsules;

public class CapsuleItemManager : BaseDomainService<CapsuleItem>, ICapsuleItemManager
{
    public CapsuleItemManager(
        ICapsuleItemRepository baseRepository,
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

    public CapsuleItem Create(CapsuleCreateModel capsuleCreateModel, Guid capsuleId)
    {
        var capsuleItem = new CapsuleItem(
            GuidGenerator.Create(),
            capsuleId,
            capsuleCreateModel.ContentType,
            capsuleCreateModel.TextContext,
            capsuleCreateModel.FileUrl,
            capsuleCreateModel.FileName
        );
        return capsuleItem;
    }
}