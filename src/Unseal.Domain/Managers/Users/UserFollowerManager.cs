using System;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Users;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Repositories.Users;

namespace Unseal.Managers.Users;

public class UserFollowerManager : BaseDomainService<UserFollower>, IUserFollowerManager
{
    public UserFollowerManager(
        IUserFollowerRepository baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer) :
        base(baseRepository,
            stringLocalizer,
            ExceptionCodes.UserFollower.NotFound,
            ExceptionCodes.UserFollower.AlreadyExists
        )
    {
    }

    public UserFollower Create(
        Guid userId,
        Guid followerId,
        Guid statusId
    )
    {
        var userFollower = new UserFollower(
            GuidGenerator.Create(),
            userId,
            followerId,
            statusId,
            DateTime.Now
        );

        return userFollower;
    }
}