using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Entities.Users;
using Unseal.Repositories.Base;

namespace Unseal.Repositories.Users;

public interface IUserViewTrackingRepository : IBaseRepository<UserViewTracking>
{
    Task<List<Guid>> UserIdsViewedProfileAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default
    );
}