using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Entities.Users;
using Unseal.Repositories.Base;

namespace Unseal.Repositories.Users;

public interface IUserProfileRepository : IBaseRepository<UserProfile>
{
    Task<Dictionary<Guid,string?>> GetProfilePictureUrlsByUserIdsAsync(HashSet<Guid> userId,
        CancellationToken cancellationToken = default);
}