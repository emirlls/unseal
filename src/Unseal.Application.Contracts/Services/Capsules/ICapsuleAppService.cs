using System.Threading;
using System.Threading.Tasks;
using Unseal.Dtos.Capsules;
using Unseal.Filtering.Capsules;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Unseal.Services.Capsules;

public interface ICapsuleAppService : IApplicationService
{
    Task<bool> CreateAsync(
        CapsuleCreateDto capsuleCreateDto,
        CancellationToken cancellationToken = default
    );

    Task<PagedResultDto<CapsuleDto>> GetFilteredListAsync(
        CapsuleFilters capsuleFilters,
        CancellationToken cancellationToken = default
    );
}