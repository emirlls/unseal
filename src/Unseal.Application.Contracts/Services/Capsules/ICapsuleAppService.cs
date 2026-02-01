using System;
using System.Collections.Generic;
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

    Task<bool> LikeAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
    Task<bool> UnLikeAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
    Task<bool> CommentAsync(
        Guid id,
        string comment,
        CancellationToken cancellationToken = default
    );
    Task<bool> UnCommentAsync(
        Guid commentId,
        CancellationToken cancellationToken = default
    );

    Task<CapsuleDetailDto> GetDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<PagedResultDto<CapsuleDto>> GetExploreFeedAsync(
        CapsuleFilters capsuleFilters,
        CancellationToken cancellationToken = default
    );

    Task<bool> MarkAsViewedAsync(
        List<Guid> capsuleIds, 
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
}