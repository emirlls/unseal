using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using StackExchange.Redis;
using Unseal.Constants;
using Unseal.Dtos.Users;
using Unseal.Enums;
using Unseal.Etos;
using Unseal.Extensions;
using Unseal.Filtering.Users;
using Unseal.Interfaces.Managers.Auth;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Models.ServerSentEvents;
using Unseal.Models.Users;
using Unseal.Repositories.Capsules;
using Unseal.Repositories.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;

namespace Unseal.Services.Users;

public class UserAppService : UnsealAppService, IUserAppService
{
    private ICustomIdentityUserManager CustomIdentityUserManager =>
        LazyServiceProvider.LazyGetRequiredService<ICustomIdentityUserManager>();

    private IUserFollowerManager UserFollowerManager =>
        LazyServiceProvider.LazyGetRequiredService<IUserFollowerManager>();

    private IUserFollowerRepository UserFollowerRepository =>
        LazyServiceProvider.LazyGetRequiredService<IUserFollowerRepository>();

    private IUserInteractionManager UserInteractionManager =>
        LazyServiceProvider.LazyGetRequiredService<IUserInteractionManager>();

    private IUserProfileManager UserProfileManager =>
        LazyServiceProvider.LazyGetRequiredService<IUserProfileManager>();

    private IUserProfileRepository UserProfileRepository =>
        LazyServiceProvider.LazyGetRequiredService<IUserProfileRepository>();

    private IUserInteractionRepository UserInteractionRepository =>
        LazyServiceProvider.LazyGetRequiredService<IUserInteractionRepository>();

    private ICapsuleRepository CapsuleRepository =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleRepository>();

    private IEsUserRepository EsUserRepository =>
        LazyServiceProvider.LazyGetRequiredService<IEsUserRepository>();

    private IUserViewTrackingRepository UserViewTrackingRepository =>
        LazyServiceProvider.LazyGetRequiredService<IUserViewTrackingRepository>();

    private IDistributedEventBus DistributedEventBus =>
        LazyServiceProvider.LazyGetRequiredService<IDistributedEventBus>();

    private IStringLocalizer<UnsealResource> StringLocalizer =>
        LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<UnsealResource>>();

    public async Task<bool> FollowAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await CustomIdentityUserManager
            .TryGetByAsync(x =>
                    x.Id.Equals(userId), throwIfNull: true,
                cancellationToken: cancellationToken);
        var userProfile = await UserProfileManager.TryGetByAsync(x =>
            x.Id.Equals(user.Id), cancellationToken: cancellationToken);
        var userFollowerStatusId =
            Guid.Parse(LookupSeederConstants.UserFollowStatusesConstants.Accepted.Id);
        if (userProfile.IsLocked)
        {
            userFollowerStatusId =
                Guid.Parse(LookupSeederConstants.UserFollowStatusesConstants.Pending.Id);
        }

        var currentUserId = CurrentUser.GetId();

        await CheckUserIsBlocked(userId, cancellationToken);
        await UserFollowerManager.TryGetByAsync(x =>
                x.UserId.Equals(user.Id) && x.FollowerId.Equals(currentUserId),
            throwIfExists: true,
            cancellationToken: cancellationToken
        );
        var userFollower = UserFollowerManager
            .Create(
                user.Id,
                currentUserId,
                userFollowerStatusId
            );
        await UserFollowerRepository.InsertAsync(userFollower, cancellationToken: cancellationToken);
        return true;
    }

    public async Task<bool> UnfollowAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await CheckUserIsBlocked(userId, cancellationToken);
        var user = await CustomIdentityUserManager
            .TryGetByAsync(x =>
                    x.Id.Equals(userId), throwIfNull: true,
                cancellationToken: cancellationToken);
        var currentUserId = CurrentUser.GetId();
        var userFollower = await UserFollowerManager.TryGetByAsync(x =>
                x.UserId.Equals(user.Id) &&
                x.FollowerId.Equals(currentUserId),
            cancellationToken: cancellationToken
        );
        await UserFollowerRepository.HardDeleteAsync(userFollower, cancellationToken);
        return true;
    }

    public async Task<bool> BlockAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var currentUserId = CurrentUser.GetId();
        var userInteraction = await UserInteractionManager.TryGetByAsync(x =>
                x.SourceUserId.Equals(currentUserId) && x.TargetUserId.Equals(userId),
            cancellationToken: cancellationToken);
        if (userInteraction is null)
        {
            var entity = UserInteractionManager.Create(currentUserId, userId);
            entity.IsBlocked = true;
            await UserInteractionRepository.InsertAsync(entity, cancellationToken: cancellationToken);
            return true;
        }

        userInteraction.IsBlocked = true;
        await UserInteractionRepository.UpdateAsync(userInteraction, cancellationToken: cancellationToken);
        var userFollowers = await UserFollowerManager.TryGetListByAsync(x =>
                (x.UserId.Equals(currentUserId) && x.FollowerId.Equals(userId)) ||
                (x.UserId.Equals(userId) && x.FollowerId.Equals(currentUserId)),
            cancellationToken: cancellationToken);
        if (!userFollowers.IsNullOrEmpty())
        {
            await UserFollowerRepository.HardDeleteManyAsync(userFollowers, cancellationToken);
        }

        var userElasticEto = new UserElasticEto
        {
            UserId = currentUserId,
            BlockedUserId = userId,
            UserElasticQueryTpe = (int)UserElasticQueryTypes.Update
        };
        await DistributedEventBus.PublishAsync(userElasticEto);
        return true;
    }

    public async Task<bool> UnBlockAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var currentUserId = CurrentUser.GetId();
        var userInteraction = await UserInteractionManager.TryGetByAsync(x =>
                x.SourceUserId.Equals(currentUserId) && x.TargetUserId.Equals(userId),
            cancellationToken: cancellationToken);

        if (userInteraction is null)
        {
            var entity = UserInteractionManager.Create(currentUserId, userId);
            await UserInteractionRepository.InsertAsync(entity, cancellationToken: cancellationToken);
            return true;
        }

        userInteraction.IsBlocked = false;
        await UserInteractionRepository.UpdateAsync(userInteraction, cancellationToken: cancellationToken);
        return true;
    }

    public async Task<UserDetailDto> GetProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var currentUser = CurrentUser.GetId();
        if (!userId.Equals(currentUser))
        {
            await CheckUserIsBlocked(userId, cancellationToken);
        }

        var userProfile = await UserProfileManager.TryGetByQueryableAsync(x => x
                .Include(x => x.User)
                .Where(c => c.UserId.Equals(userId)),
            throwIfNull: true,
            cancellationToken: cancellationToken
        );
        var capsuleUrls = await CapsuleRepository
            .GetCapsuleUrlsByUserIdAsync(
                userId,
                cancellationToken
            );
        var followCounts = await UserFollowerRepository
            .GetFollowCountsAsync(
                userId,
                cancellationToken
            );
        var decryptedProfilePictureUrl = LazyServiceProvider
            .GetDecryptedFileUrlAsync(userProfile.ProfilePictureUrl);
        var response = new UserDetailDto
        {
            UserDto = new UserDto
            {
                Id = userId,
                Username = userProfile.User.UserName,
                ProfilePictureUrl = decryptedProfilePictureUrl
            },
            CapsuleUrls = capsuleUrls,
            FollowerCount = followCounts.followerCount,
            FollowCount = followCounts.followCount,
            LastActivity = userProfile.LastActivityTime
        };
        return response;
    }

    public async Task<bool> AcceptFollowRequestAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        await UpdateUserFollowStatusAsync(
            userId,
            Guid.Parse(LookupSeederConstants.UserFollowStatusesConstants.Accepted.Id),
            cancellationToken);

        return true;
    }

    public async Task<bool> RejectFollowRequestAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await UpdateUserFollowStatusAsync(
            userId,
            Guid.Parse(LookupSeederConstants.UserFollowStatusesConstants.Rejected.Id),
            cancellationToken);
        return true;
    }

    public async Task<PagedResultDto<UserDto>> GetFollowRequestsAsync(
        CancellationToken cancellationToken = default
    )
    {
        var pendingUserFollowers = await UserFollowerManager.TryGetListByAsync(x =>
                x.UserId.Equals(CurrentUser.GetId()) &&
                x.StatusId == Guid.Parse(LookupSeederConstants.UserFollowStatusesConstants.Pending.Id),
            cancellationToken: cancellationToken
        );
        var pendingUserIds = pendingUserFollowers
            .Select(x => x.FollowerId)
            .ToHashSet();

        var userProfiles = await UserProfileRepository
            .GetDynamicListAsync(new UserProfileFilters(),
                q => q
                    .Include(x => x.User)
                    .Where(x => pendingUserIds.Contains(x.Id)
                    ), cancellationToken: cancellationToken);

        var count = await UserProfileRepository
            .GetDynamicListCountAsync(
                new UserProfileFilters(),
                q => q
                    .Where(x => pendingUserIds.Contains(x.Id)),
                cancellationToken: cancellationToken
            );


        var userDto = userProfiles
            .Select(x =>
            {
                var decryptedProfilePictureUrl = LazyServiceProvider
                    .GetDecryptedFileUrlAsync(x.ProfilePictureUrl);
                return new UserDto
                {
                    Id = x.UserId!,
                    Username = x.User.UserName,
                    ProfilePictureUrl = decryptedProfilePictureUrl
                };
            })
            .ToList();

        var response = new PagedResultDto<UserDto>
        {
            Items = userDto,
            TotalCount = count
        };
        return response;
    }

    public async Task<PagedResultDto<UserDto>> GetFollowersAsync(
        CancellationToken cancellationToken = default
    )
    {
        var userFollowers = await UserFollowerManager.TryGetListByAsync(x =>
                x.UserId.Equals(CurrentUser.GetId()) &&
                x.StatusId == Guid.Parse(LookupSeederConstants.UserFollowStatusesConstants.Accepted.Id),
            cancellationToken: cancellationToken
        );
        var followerIds = userFollowers
            .Select(x => x.FollowerId)
            .ToHashSet();

        var userProfiles = await UserProfileRepository
            .GetDynamicListAsync(new UserProfileFilters
                {
                    SkipCount = 0,
                    MaxResultCount = 100,
                },
                q => q
                    .Include(x => x.User)
                    .Where(x => followerIds.Contains(x.Id)
                    ), cancellationToken: cancellationToken);

        var count = await UserProfileRepository
            .GetDynamicListCountAsync(
                new UserProfileFilters(),
                q => q
                    .Where(x => followerIds.Contains(x.Id)),
                cancellationToken: cancellationToken
            );

        var userDto = userProfiles
            .Select(x =>
            {
                var decryptedProfilePictureUrl = LazyServiceProvider
                    .GetDecryptedFileUrlAsync(x.ProfilePictureUrl);
                return new UserDto
                {
                    Id = x.UserId!,
                    Username = x.User.UserName,
                    ProfilePictureUrl = decryptedProfilePictureUrl
                };
            })
            .ToList();

        var response = new PagedResultDto<UserDto>
        {
            Items = userDto,
            TotalCount = count
        };
        return response;
    }

    public async Task<bool> UpdateProfileAsync(
        UserProfileUpdateDto userProfileUpdateDto,
        CancellationToken cancellationToken = default
    )
    {
        var currentUserId = CurrentUser.GetId();
        var userProfile = await UserProfileManager.TryGetByAsync(x =>
            x.UserId.Equals(currentUserId), cancellationToken: cancellationToken);
        var userProfileUpdateModel = new UserProfileUpdateModel(
            userProfileUpdateDto.IsLocked,
            userProfileUpdateDto.AllowJoinGroup,
            userProfileUpdateDto.Content,
            null
        );
        if (userProfileUpdateDto.StreamContent != null)
        {
            var fileUrl = await LazyServiceProvider.UploadFileAsync(userProfileUpdateDto.StreamContent);
            var encryptedFileUrl = LazyServiceProvider.GetEncryptedFileUrlAsync(fileUrl);
            userProfileUpdateModel = userProfileUpdateModel with { ProfilePictureUrl = encryptedFileUrl };

            var userElasticEto = new UserElasticEto
            {
                UserId = userProfile.UserId,
                ProfilePictureUrl = encryptedFileUrl,
                UserElasticQueryTpe = (int)UserElasticQueryTypes.Update
            };
            await DistributedEventBus.PublishAsync(userElasticEto);
        }

        UserProfileManager.UpdateUserProfile(userProfile, userProfileUpdateModel);
        await UserProfileRepository.UpdateAsync(userProfile, cancellationToken: cancellationToken);
        return true;
    }

    public async Task<PagedResultDto<UserDto>> GetBlockedUsersAsync(
        UserInteractionFilters filters,
        CancellationToken cancellationToken = default
    )
    {
        filters.SetSourceUserId(CurrentUser.GetId());
        var blockedUsers = await UserInteractionRepository.GetDynamicListAsync(
            filters,
            null,
            asNoTracking: true,
            cancellationToken: cancellationToken
        );
        var blockedUserCount = await UserInteractionRepository.GetDynamicListCountAsync(
            filters,
            null,
            cancellationToken: cancellationToken
        );
        var blockedUserIds = blockedUsers
            .Select(x => x.TargetUserId)
            .ToHashSet();

        var userProfilePictureUrls =
            await UserProfileRepository.GetProfilePictureUrlsByUserIdsAsync(blockedUserIds, cancellationToken);
        var userDtos = blockedUsers.Select(x =>
            {
                var targetProfilePictureUrl = userProfilePictureUrls
                    .GetValueOrDefault(x.TargetUserId)?
                    .ToString();
                var decryptedProfilePictureUrl = LazyServiceProvider
                    .GetDecryptedFileUrlAsync(targetProfilePictureUrl);
                return new UserDto
                {
                    Id = x.TargetUserId,
                    Username = x.TargetUser.UserName,
                    ProfilePictureUrl = decryptedProfilePictureUrl
                };
            })
            .ToList();
        var response = new PagedResultDto<UserDto>
        {
            Items = userDtos,
            TotalCount = blockedUserCount
        };
        return response;
    }

    public async Task<PagedResultDto<UserDto>> SearchAsync(
        string userName,
        CancellationToken cancellationToken = default
    )
    {
        var currentUserId = CurrentUser.GetId();
        var blockedUserIds = await UserInteractionManager
            .GetUserBlockedUserIdsAsync(currentUserId, cancellationToken);
        var users = await EsUserRepository.SearchUsersAsync(
            blockedUserIds,
            currentUserId,
            userName,
            cancellationToken: cancellationToken);
        var userDtos = users.Select(x =>
            {
                var profilePictureUrl = LazyServiceProvider.GetDecryptedFileUrlAsync(x.ProfilePictureUrl);
                return new UserDto
                {
                    Id = x.Id,
                    Username = x.UserName,
                    ProfilePictureUrl = profilePictureUrl
                };
            })
            .ToList();

        var response = new PagedResultDto<UserDto>
        {
            Items = userDtos,
            TotalCount = userDtos.Count
        };
        return response;
    }

    public async Task<PagedResultDto<UserDto>> GetUsersViewedProfileAsync(
        UserProfileFilters filters,
        CancellationToken cancellationToken = default
    )
    {
        var viewedUserIds = await UserViewTrackingRepository.UserIdsViewedProfileAsync(
            CurrentUser.GetId(),
            cancellationToken
        );
        var userProfiles = await UserProfileRepository
            .GetDynamicListAsync(filters,
                q => q
                    .Include(x => x.User)
                    .Where(x => viewedUserIds.Contains(x.Id)
                    ), cancellationToken: cancellationToken);

        var count = await UserProfileRepository
            .GetDynamicListCountAsync(
                new UserProfileFilters(),
                q => q
                    .Where(x => viewedUserIds.Contains(x.Id)),
                cancellationToken: cancellationToken
            );

        var userDto = userProfiles
            .Select(x =>
            {
                var decryptedProfilePictureUrl = LazyServiceProvider
                    .GetDecryptedFileUrlAsync(x.ProfilePictureUrl);
                return new UserDto
                {
                    Id = x.UserId!,
                    Username = x.User.UserName,
                    ProfilePictureUrl = decryptedProfilePictureUrl
                };
            })
            .ToList();

        var response = new PagedResultDto<UserDto>
        {
            Items = userDto,
            TotalCount = count
        };
        return response;
    }

    private async Task UpdateUserFollowStatusAsync(
        Guid userId,
        Guid newStatusId,
        CancellationToken cancellationToken = default)
    {
        var user = await CustomIdentityUserManager
            .TryGetByAsync(x =>
                    x.Id.Equals(userId), throwIfNull: true,
                cancellationToken: cancellationToken);

        await CheckUserIsBlocked(userId, cancellationToken);
        var userFollower = await UserFollowerManager.TryGetByAsync(x =>
                x.UserId.Equals(user.Id) && x.FollowerId.Equals(CurrentUser.GetId()),
            throwIfNull: true,
            cancellationToken: cancellationToken
        );
        userFollower.StatusId = newStatusId;
        await UserFollowerRepository.UpdateAsync(userFollower, cancellationToken: cancellationToken);
        await FollowRequestAcceptPubAsync(userId, newStatusId, cancellationToken);
    }

    private async Task FollowRequestAcceptPubAsync(
        Guid userId,
        Guid newStatusId,
        CancellationToken cancellationToken = default
    )
    {
        if (newStatusId == Guid.Parse(LookupSeederConstants.UserFollowStatusesConstants.Accepted.Id))
        {
            var redis = LazyServiceProvider.GetRequiredService<IConnectionMultiplexer>();
            var subscriber = redis.GetSubscriber();

            var currentUser = await CustomIdentityUserManager
                .TryGetByAsync(x =>
                        x.Id.Equals(CurrentUser.GetId()), throwIfNull: true,
                    cancellationToken: cancellationToken);

            var eventModel = new FollowRequestAcceptedEventModel
            {
                FollowerUserId = userId,
                UserId = currentUser.Id,
                UserName = currentUser.UserName,
                CreationTime = DateTime.Now
            };
            var payload = JsonSerializer.Serialize(eventModel);

            await subscriber.PublishAsync(
                EventConstants.ServerSentEvents.FollowRequestAccept.FollowRequestAcceptChannel,
                payload
            );
        }
    }

    private async Task CheckUserIsBlocked(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var isCurrentUserBlocked = await UserInteractionManager
            .CheckUserBlockedAsync(
                userId,
                CurrentUser.GetId(),
                cancellationToken
            );
        var isTargetUserBlocked = await UserInteractionManager
            .CheckUserBlockedAsync(
                CurrentUser.GetId(),
                userId,
                cancellationToken
            );
        if (isCurrentUserBlocked || isTargetUserBlocked)
        {
            throw new UserFriendlyException(StringLocalizer[ExceptionCodes.UserFollower.UserIsBanned]);
        }
    }
}