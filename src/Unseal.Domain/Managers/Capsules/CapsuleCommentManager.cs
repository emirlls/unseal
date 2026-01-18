using System;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Localization;
using Unseal.Repositories.Base;

namespace Unseal.Managers.Capsules;

public class CapsuleCommentManager : BaseDomainService<CapsuleComment>, ICapsuleCommentManager
{
    public CapsuleCommentManager(
        IBaseRepository<CapsuleComment> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
    ) : base(
        baseRepository,
        stringLocalizer,
        ExceptionCodes.CapsuleComment.NotFound,
        ExceptionCodes.CapsuleComment.AlreadyExists
    )
    {
    }

    public CapsuleComment Create(Guid capsuleId, Guid? currentUserId, string comment)
    {
        var capsuleComment =
            new CapsuleComment(
                GuidGenerator.Create(),
                capsuleId,
                (Guid)currentUserId,
                comment,
                DateTime.Now
            );
        return capsuleComment;
    }
}