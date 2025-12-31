using System;
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
}