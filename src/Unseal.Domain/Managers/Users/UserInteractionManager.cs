using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Users;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Repositories.Users;

namespace Unseal.Managers.Users;

public class UserInteractionManager : BaseDomainService<UserInteraction>, IUserInteractionManager
{
    public UserInteractionManager(
        IUserInteractionRepository baseRepository,
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
            await ExistsAsync(x =>
                    x.SourceUserId.Equals(sourceUserId) &&
                    x.TargetUserId.Equals(targetUserId) &&
                    x.IsBlocked,
                throwIfNotExists: false,
                cancellationToken
            );
        return isBlocked;
    }

    public UserInteraction Create(Guid sourceUserId, Guid targetUserId)
    {
        var entity = new UserInteraction(GuidGenerator.Create(), sourceUserId, targetUserId);
        return entity;
    }

    public async Task<List<Guid>>? GetUserIdsBlockedUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var response = new List<Guid>();
        var userInteractions = (await _baseRepository.TryGetQueryableAsync(q => q
                .Where(x => x.TargetUserId.Equals(userId)),
            cancellationToken: cancellationToken));

        if (userInteractions.Any() && userInteractions.Count() != 0)
        {
            response = userInteractions
                .Select(c => c.SourceUserId)
                .ToList();
        }

        return response;
    }
}