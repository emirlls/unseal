using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Models.ElasticSearch;
using Unseal.Repositories.Base;

namespace Unseal.Repositories.Users;

public interface IEsUserRepository : IElasticSearchRepository<UserElasticModel, Guid>
{
    Task AddBlockedUserAsync(Guid targetUserId, Guid blockedByUserId);

    Task UpdateProfilePictureAsync(Guid userId, string? newUrl);
    Task<List<UserElasticModel>> SearchUsersAsync(
        List<Guid>? blockedUserIds,
        Guid currentUserId,
        string searchText,
        int size = 10,
        CancellationToken cancellationToken = default
    );
}