using System;
using Unseal.Entities.Users;

namespace Unseal.Interfaces.Managers.Users;

public interface IUserFollowerManager : IBaseDomainService<UserFollower>
{
    UserFollower Create(
        Guid userId,
        Guid followerId,
        Guid statusId
    );
}