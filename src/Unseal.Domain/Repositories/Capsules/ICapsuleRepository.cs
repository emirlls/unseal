using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Entities.Capsules;
using Unseal.Filtering.Capsules;
using Unseal.Repositories.Base;

namespace Unseal.Repositories.Capsules;

public interface ICapsuleRepository : IBaseRepository<Capsule>
{
    Task<List<Capsule>> GetFilteredListAsync(
        CapsuleFilters capsuleFilters,
        CancellationToken cancellationToken = default
    );
    Task<long> GetFilteredCountAsync(
        CapsuleFilters capsuleFilters,
        CancellationToken cancellationToken = default
    );
}