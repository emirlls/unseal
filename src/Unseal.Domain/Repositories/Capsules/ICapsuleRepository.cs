using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Entities.Capsules;
using Unseal.Repositories.Base;

namespace Unseal.Repositories.Capsules;

public interface ICapsuleRepository : IBaseRepository<Capsule>
{
    Task<List<string?>> GetCapsuleUrlsByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default);
}