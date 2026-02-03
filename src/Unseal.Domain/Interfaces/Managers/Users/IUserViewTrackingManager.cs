using System;
using System.Collections.Generic;
using Unseal.Entities.Users;

namespace Unseal.Interfaces.Managers.Users;

public interface IUserViewTrackingManager : IBaseDomainService<UserViewTracking>
{
    List<UserViewTracking> Create(
        List<Guid> capsuleIds,
        Guid userId
    );
}