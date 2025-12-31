using System;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Dtos.Groups;
using Unseal.Filtering.Users;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Unseal.Services.Groups;

public interface IGroupAppService : IApplicationService
{
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

    Task<GroupDetailDto> GetDetailAsync(
        Guid groupId,
        CancellationToken cancellationToken = default);

    Task<bool> LeaveAsync(
        Guid groupId,
        CancellationToken cancellationToken = default
    );
}