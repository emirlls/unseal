using System;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Dtos.Users;
using Unseal.Filtering.Users;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Unseal.Services.Users;

public interface IUserAppService : IApplicationService
{
    Task<bool> FollowAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    Task<bool> UnfollowAsync(
        Guid userId, 
        CancellationToken cancellationToken = default
    );

    Task<bool> CreateGroupAsync(
        GroupCreateDto groupCreateDto,
        CancellationToken cancellationToken = default
    );

    Task<bool> UpdateGroupAsync(
        Guid groupId,
        GroupUpdateDto groupUpdateDto,
        CancellationToken cancellationToken = default
    );

    Task<PagedResultDto<GroupDto>> GetFilteredGroupListAsync(
        GroupFilters groupFilters,
        CancellationToken cancellationToken = default
    );
}