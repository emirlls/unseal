using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Entities.Capsules;
using Unseal.Models.Dashboards;
using Unseal.Repositories.Base;

namespace Unseal.Repositories.Capsules;

public interface ICapsuleRepository : IBaseRepository<Capsule>
{
    /// <summary>
    /// Returns file urls of the capsules created by <paramref name="userId"/> that <paramref name="viewerId"/> can see.
    /// </summary>
    Task<List<string?>> GetCapsuleUrlsByUserIdAsync(Guid userId,
        Guid viewerId,
        CancellationToken cancellationToken = default);

    Task<List<CreationCapsuleByDateModel>> GetCreationCapsuleByDateAsync(
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default
    );
    
}