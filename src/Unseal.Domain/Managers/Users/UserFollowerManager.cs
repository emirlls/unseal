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

public class UserFollowerManager : BaseDomainService<UserFollower>, IUserFollowerManager
{
    public UserFollowerManager(IBaseRepository<UserFollower> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer) :
        base(baseRepository,
            stringLocalizer,
            ExceptionCodes.UserFollower.NotFound,
            ExceptionCodes.UserFollower.AlreadyExists
        )
    {
    }

    public UserFollower Create(Guid userId, Guid followerId)
    {
        var userFollower = new UserFollower(
            GuidGenerator.Create(),
            userId,
            followerId
        );

        return userFollower;
    }

    public async Task<bool> CheckUserBlockedAsync(
        Guid? userId,
        Guid followerId,
        CancellationToken cancellationToken = default
    )
    {
        var isBlocked =
            await ExistsAsync(x => x.UserId.Equals(userId) && x.FollowerId.Equals(followerId) && x.IsBlocked,
                cancellationToken);
        return isBlocked;
    }
}