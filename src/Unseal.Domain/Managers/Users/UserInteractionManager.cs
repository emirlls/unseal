using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Users;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Repositories.Base;

namespace Unseal.Managers.Users;

public class UserInteractionManager : BaseDomainService<UserInteraction>, IUserInteractionManager
{
    public UserInteractionManager(IBaseRepository<UserInteraction> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
    ) 
        : base(baseRepository, stringLocalizer, 
            ExceptionCodes.UserInteraction.NotFound, 
            ExceptionCodes.UserInteraction.AlreadyExists
        )
    {
    }
    public async Task<bool> CheckUserBlockedAsync(
        Guid? sourceUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default
    )
    {
        var isBlocked =
            await ExistsAsync(x => x.SourceUserId.Equals(sourceUserId) && x.TargetUserId.Equals(targetUserId) && x.IsBlocked,
                cancellationToken);
        return isBlocked;
    }

    public UserInteraction Create(Guid sourceUserId, Guid targetUserId)
    {
        var entity = new UserInteraction(GuidGenerator.Create(), sourceUserId, targetUserId);
        return entity;
    }
}