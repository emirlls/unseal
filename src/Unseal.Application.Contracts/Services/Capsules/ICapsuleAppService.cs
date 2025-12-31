using System;
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
        bool isAll = true,
        CancellationToken cancellationToken = default
    );

    Task<string> GetQrCodeAsync(
        Guid capsuleId,
        CancellationToken cancellationToken = default
    );
}