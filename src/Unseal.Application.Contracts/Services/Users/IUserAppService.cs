using System;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Dtos.Users;
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

    Task<bool> BlockAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    Task<bool> UnBlockAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    Task<UserDetailDto> GetProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    Task<bool> AcceptFollowRequestAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
    Task<bool> RejectFollowRequestAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    Task<PagedResultDto<UserDto>> GetFollowRequestsAsync(
        CancellationToken cancellationToken = default
    );
    Task<PagedResultDto<UserDto>> GetFollowersAsync(
        CancellationToken cancellationToken = default
    );

    Task<bool> UpdateProfileAsync(
        UserProfileUpdateDto userProfileUpdateDto,
        CancellationToken cancellationToken = default
    );

    Task<PagedResultDto<UserDto>> GetBlockedUsersAsync(
        CancellationToken cancellationToken = default
    );

    Task<PagedResultDto<UserDto>> SearchAsync(
        string userName,
        CancellationToken cancellationToken = default
    );
}