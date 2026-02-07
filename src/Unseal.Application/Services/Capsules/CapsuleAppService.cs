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
using Unseal.Dtos.Capsules;
using Unseal.Entities.Capsules;
using Unseal.Entities.Users;
using Unseal.Enums;
using Unseal.Extensions;
using Unseal.Filtering.Capsules;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Models.ServerSentEvents;
using Unseal.Profiles.Capsules;
using Unseal.Repositories.Capsules;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;

namespace Unseal.Services.Capsules;

public class CapsuleAppService : UnsealAppService, ICapsuleAppService
{
    private CapsuleMapper CapsuleMapper =>
        LazyServiceProvider.LazyGetRequiredService<CapsuleMapper>();

    private ICapsuleManager CapsuleManager =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleManager>();

    private ICapsuleItemManager CapsuleItemManager =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleItemManager>();

    private ICapsuleItemRepository CapsuleItemRepository =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleItemRepository>();

    private ICapsuleRepository CapsuleRepository =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleRepository>();

    private IUserInteractionManager UserInteractionManager =>
        LazyServiceProvider.LazyGetRequiredService<IUserInteractionManager>();

    private ICapsuleLikeManager CapsuleLikeManager =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleLikeManager>();

    private ICapsuleCommentManager CapsuleCommentManager =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleCommentManager>();

    private ICapsuleLikeRepository CapsuleLikeRepository =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleLikeRepository>();

    private ICapsuleCommentRepository CapsuleCommentRepository =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleCommentRepository>();

    private ICapsuleMapFeatureRepository CapsuleMapFeatureRepository =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleMapFeatureRepository>();

    private ICapsuleMapFeatureManager CapsuleMapFeatureManager =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleMapFeatureManager>();

    private IUserProfileManager UserProfileManager =>
        LazyServiceProvider.LazyGetRequiredService<IUserProfileManager>();

    private IUserViewTrackingManager UserViewTrackingManager =>
        LazyServiceProvider.LazyGetRequiredService<IUserViewTrackingManager>();
    
    private IDistributedEventBus DistributedEventBus =>
        LazyServiceProvider.LazyGetRequiredService<IDistributedEventBus>();
    
    private IStringLocalizer<UnsealResource> StringLocalizer =>
        LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<UnsealResource>>();

    public async Task<bool> CreateAsync(CapsuleCreateDto capsuleCreateDto,
        CancellationToken cancellationToken = default)
    {
        var fileUrl = await LazyServiceProvider.UploadFileAsync(capsuleCreateDto.StreamContent);
        var encryptedFileUrl = LazyServiceProvider.GetEncryptedFileUrlAsync(fileUrl);
        var capsuleCreateModel = CapsuleMapper.MaptoModel(capsuleCreateDto);
        var capsule = CapsuleManager.Create(capsuleCreateModel, CurrentUser.Id!);
        capsuleCreateModel.FileUrl = encryptedFileUrl;
        var capsuleItem = CapsuleItemManager.Create(capsuleCreateModel, capsule.Id);
        await CapsuleRepository.InsertAsync(capsule, cancellationToken: cancellationToken);
        await CapsuleItemRepository.InsertAsync(capsuleItem, cancellationToken: cancellationToken);
        await CreateCapsuleMapFeatureAsync(capsule.Id, capsuleCreateModel.GeoJson, cancellationToken);
        var redis = LazyServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        var subscriber = redis.GetSubscriber();

        var userProfile = (await UserProfileManager
            .TryGetQueryableAsync(x => x
                    .Include(c => c.User)
                    .Where(c => c.UserId.Equals(CurrentUser.GetId())),
                asNoTracking : true,
                cancellationToken: cancellationToken
            ))!.FirstOrDefault();

        var decryptedProfilePictureUrl =
            LazyServiceProvider.GetDecryptedFileUrlAsync(userProfile?.ProfilePictureUrl);

        var eventModel = new CapsuleCreatedEventModel
        {
            Id = capsule.Id,
            CreatorId = (Guid)capsule.CreatorId!,
            Name = capsule.Name,
            Username = userProfile?.User.UserName,
            FileUrl = fileUrl,
            ProfilePictureUrl = decryptedProfilePictureUrl,
            RevealDate = capsule.RevealDate,
            CreationTime = capsule.CreationTime
        };
        var payload = JsonSerializer.Serialize(eventModel);

        await subscriber.PublishAsync(
            EventConstants.ServerSentEvents.CapsuleCreate.GlobalFeedUpdateChannel,
            payload
        );
        return true;
    }

    private async Task CreateCapsuleMapFeatureAsync(
        Guid capsuleId,
        string? geoJson,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(geoJson))
        {
            return;
        }

        var capsuleMapFeature = CapsuleMapFeatureManager.Create(capsuleId, geoJson);
        await CapsuleMapFeatureRepository.InsertAsync(capsuleMapFeature, cancellationToken: cancellationToken);
    }

    public async Task<PagedResultDto<CapsuleDto>> GetFilteredListAsync(
        CapsuleFilters capsuleFilters,
        bool isAll = true,
        CancellationToken cancellationToken = default
    )
    {
        Func<IQueryable<Capsule>, IQueryable<Capsule>>? queryBuilder = null;
        if (!isAll)
        {
            queryBuilder = capsule => capsule
                .Include(x => x.CapsuleType)
                .Include(x => x.CapsuleItems)
                .Where(c => (Guid)c.CreatorId == CurrentUser.GetId());
        }
        else
        {
            capsuleFilters.SetIsOpened(true);
            capsuleFilters.SetIsActive(true);
            capsuleFilters.SetIsPublic(Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Public.Id));
            
            var usersBlockedCurrentUser = await UserInteractionManager
                .TryGetQueryableAsync(x =>
                        x.Where(c =>
                            c.TargetUserId.Equals(CurrentUser.Id) && c.IsBlocked),
                    asNoTracking : true,
                    cancellationToken: cancellationToken);
            var capsuleCreators =
                usersBlockedCurrentUser != null && usersBlockedCurrentUser.Any()
                    ? usersBlockedCurrentUser
                        .Select(x => x.SourceUserId)
                        .ToHashSet()
                    : null;
            queryBuilder = capsule => capsule
                .Include(x => x.CapsuleType)
                .Include(x => x.CapsuleItems)
                .WhereIf(!capsuleCreators.IsNullOrEmpty(),
                    x => !capsuleCreators.Contains((Guid)x.CreatorId!));
        }

        var capsules = await CapsuleRepository
            .GetDynamicListAsync(
                capsuleFilters,
                queryBuilder,
                true,
                cancellationToken
            );

        var count = await CapsuleRepository
            .GetDynamicListCountAsync(
                capsuleFilters,
                null,
                useCache: false,
                cancellationToken
            );
        var capsuleCreatorIds = capsules
            .Select(x => (Guid)x.CreatorId!)
            .ToHashSet();
        var userProfiles = await UserProfileManager.TryGetListByQueryableAsync(q => q
                .Include(x => x.User)
                .Where(x => capsuleCreatorIds.Contains(x.UserId)),
            asNoTracking: true,
            cancellationToken: cancellationToken);

        var dto = capsules.Select(x =>
            {
                var capsuleType = CapsuleMapper.ResolveType(x.CapsuleType);
                var userProfile = userProfiles.FirstOrDefault(u => u.UserId.Equals(x.CreatorId));
                var decryptedProfilePictureUrl =
                    LazyServiceProvider.GetDecryptedFileUrlAsync(userProfile?.ProfilePictureUrl);
                var fileUrl =
                    LazyServiceProvider.GetDecryptedFileUrlAsync(x.CapsuleItems.FileUrl);
                return new CapsuleDto(
                    x.Id,
                    (Guid)x.CreatorId!,
                    x.Name,
                    capsuleType,
                    userProfile.User.UserName,
                    decryptedProfilePictureUrl,
                    fileUrl,
                    x.RevealDate,
                    x.CreationTime);
            })
            .ToList();
        var response = new PagedResultDto<CapsuleDto>
        {
            Items = dto,
            TotalCount = count
        };
        return response;
    }

    public async Task<string> GetQrCodeAsync(
        Guid capsuleId,
        CancellationToken cancellationToken = default
    )
    {
        var capsule = await CapsuleManager.TryGetByAsync(x =>
                x.Id.Equals(capsuleId), true,
            cancellationToken: cancellationToken);
        if (capsule.CapsuleTypeId != Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Public.Id))
        {
            throw new UserFriendlyException(StringLocalizer[ExceptionCodes.Capsule.CanNotMakeQrNonPublic]);
        }

        var base64QrCode = await LazyServiceProvider.GenerateQrCodeAsync(capsuleId);
        return base64QrCode;
    }

    public async Task<bool> LikeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var capsule = await CapsuleManager.TryGetByAsync(x =>
                x.Id.Equals(id), true,
            cancellationToken: cancellationToken);

        var capsuleLike = CapsuleLikeManager.Create(capsule!.Id, CurrentUser.Id);
        await CapsuleLikeRepository.InsertAsync(capsuleLike, cancellationToken: cancellationToken);
        return true;
    }

    public async Task<bool> UnLikeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var capsule = await CapsuleManager.TryGetByAsync(x =>
                x.Id.Equals(id), true,
            cancellationToken: cancellationToken);
        var capsuleLike = await CapsuleLikeManager.TryGetByAsync(x =>
                x.CapsuleId.Equals(capsule!.Id) && x.UserId.Equals(CurrentUser.Id),
            true, cancellationToken: cancellationToken);
        await CapsuleLikeRepository.HardDeleteAsync(capsuleLike!, cancellationToken);
        return true;
    }

    public async Task<bool> CommentAsync(
        Guid id,
        string comment,
        CancellationToken cancellationToken = default
    )
    {
        var capsule = await CapsuleManager.TryGetByAsync(x =>
                x.Id.Equals(id), true,
            cancellationToken: cancellationToken);
        var capsuleComment = CapsuleCommentManager.Create(capsule.Id, CurrentUser.Id, comment);
        await CapsuleCommentRepository.InsertAsync(capsuleComment, cancellationToken: cancellationToken);
        return true;
    }

    public async Task<bool> UnCommentAsync(
        Guid commentId,
        CancellationToken cancellationToken = default
    )
    {
        var comment = await CapsuleCommentManager.TryGetByAsync(x =>
                x.Id.Equals(commentId),
            true,
            cancellationToken: cancellationToken);
        await CapsuleCommentRepository.HardDeleteAsync(comment, cancellationToken);
        return true;
    }

    public async Task<CapsuleDetailDto> GetDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var capsule = await (await CapsuleManager.TryGetQueryableAsync(q => q
                    .Include(x => x.CapsuleItems)
                    .Include(x => x.CapsuleType)
                    .Include(x => x.CapsuleMapFeatures)
                    .Include(x => x.CapsuleComments)
                    .ThenInclude(c => c.User)
                    .Include(x => x.CapsuleLikes)
                    .ThenInclude(c => c.User)
                    .Where(x => x.Id.Equals(id)),
                true,
                cancellationToken: cancellationToken))!
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var blockedUserIds = (await UserInteractionManager
                .GetUserIdsBlockedUserAsync(CurrentUser.GetId(), cancellationToken)!)?
            .ToHashSet() ?? new HashSet<Guid>();

        var validLikes = capsule?.CapsuleLikes
            .Where(l => !blockedUserIds.Contains(l.UserId))
            .ToList();

        var validComments = capsule?
            .CapsuleComments
            .Where(c => !blockedUserIds.Contains(c.UserId))
            .ToList();

        var allUserIds = (validLikes ?? Enumerable.Empty<CapsuleLike>())
            .Select(l => l.UserId)
            .Union((validComments ?? Enumerable.Empty<CapsuleComment>())
                .Select(c => c.UserId))
            .Append(capsule!.CreatorId ?? Guid.Empty)
            .ToHashSet();

        var profiles = (await UserProfileManager.TryGetListByQueryableAsync(q => q
                    .Include(p => p.User)
                    .Where(p => allUserIds.Contains(p.UserId)),
                cancellationToken: cancellationToken))
            .ToDictionary(p => p.UserId);

        var creatorProfile = profiles.GetValueOrDefault(capsule.CreatorId ?? Guid.Empty);
        var decryptedProfilePictureUrl =
            LazyServiceProvider.GetDecryptedFileUrlAsync(creatorProfile?.ProfilePictureUrl);
        var decryptedFileUrl = LazyServiceProvider.GetDecryptedFileUrlAsync(capsule.CapsuleItems.FileUrl);

        var response = new CapsuleDetailDto
        {
            Id = capsule.Id,
            CreatorId = (Guid)capsule.CreatorId!,
            CreatorUserName = creatorProfile?.User.UserName,
            Name = capsule.Name,
            Type = ((CapsuleTypes)capsule.CapsuleType.Code).GetDescription(),
            CreatorProfilePictureUrl = decryptedProfilePictureUrl,
            FileUrl = decryptedFileUrl,
            RevealDate = capsule.RevealDate,
            CreationTime = capsule.CreationTime,
            LikeDtos = await MapLikesAsync(validLikes, profiles),
            CommentDtos = await MapCommentsAsync(validComments, profiles)
        };
        return response;
    }

    public async Task<PagedResultDto<CapsuleDto>> GetExploreFeedAsync(
        CapsuleFilters capsuleFilters,
        CancellationToken cancellationToken = default
    )
    {
        var currentUserId = CurrentUser.GetId();
        var now = DateTime.UtcNow;

        var usersBlockedCurrentUser = await UserInteractionManager
            .TryGetQueryableAsync(x => x
                    .Where(c => c.TargetUserId.Equals(currentUserId) && c.IsBlocked),
                asNoTracking : true,
                cancellationToken: cancellationToken);

        var blockedCreatorIds = usersBlockedCurrentUser?
            .Select(x => x.SourceUserId)
            .ToHashSet();

        var queryable = await UserViewTrackingManager.TryGetQueryableAsync(q => q
                .Include(x => x.UserViewTrackingType)
                .Where(x => x.UserId == currentUserId &&
                            x.UserViewTrackingType.Code == (int)UserViewTrackingTypes.Capsule),
            asNoTracking : true,
            cancellationToken: cancellationToken
        );

        var viewedIds = queryable != null
            ? await queryable
                .Select(v => v.ExternalId)
                .ToListAsync(cancellationToken)
            : new List<Guid>();

        var currentUserLikesQueryable = await CapsuleLikeManager
            .TryGetQueryableAsync(q => q
                    .Where(x => x.UserId == currentUserId),
                asNoTracking : true,
                cancellationToken: cancellationToken
            );

        var currentUserLikedIds = currentUserLikesQueryable != null
            ? await currentUserLikesQueryable
                .OrderByDescending(x => x.CreationTime)
                .Take(10)
                .Select(x => x.CapsuleId)
                .ToListAsync(cancellationToken)
            : new List<Guid>();

        var likedOtherUsersQuery = await CapsuleLikeManager.TryGetQueryableAsync(
            q => q.Where(x => currentUserLikedIds.Contains(x.CapsuleId) && x.UserId != currentUserId),
            asNoTracking : true,
            cancellationToken: cancellationToken);

        var likedOtherUserIds = likedOtherUsersQuery != null
            ? await likedOtherUsersQuery
                .GroupBy(x => x.UserId)
                .OrderByDescending(g => g.Count())
                .Take(50)
                .Select(g => g.Key)
                .ToListAsync(cancellationToken)
            : new List<Guid>();

        var query = await CapsuleManager.TryGetQueryableAsync(q => q
                .Include(x => x.CapsuleType)
                .Include(x => x.CapsuleItems)
                .Where(x => (bool)x.IsOpened!)
                .Where(x => (bool)x.IsActive!)
                .Where(x => x.CapsuleType.Code == (int)CapsuleTypes.Public)
                .Where(x => !viewedIds.Contains(x.Id))
                .WhereIf(!blockedCreatorIds.IsNullOrEmpty(),
                    x => !blockedCreatorIds!.Contains((Guid)x.CreatorId!)),
            asNoTracking: true,
            cancellationToken: cancellationToken
        );
        if (query is null)
        {
            return new PagedResultDto<CapsuleDto>();
        }

        var count = await query.CountAsync(cancellationToken);
        var rawCapsules = await query
            .Select(c => new
            {
                Capsule = c,
                LikeCount = c.CapsuleLikes.Count,
                CommentCount = c.CapsuleComments.Count,
                IsLikedOtherUsers = c.CapsuleLikes.Any(i => likedOtherUserIds.Contains(i.UserId)),
                CreationTime = c.CreationTime
            })
            .ToListAsync(cancellationToken)!;
        var capsulesWithScores = rawCapsules
            .Select(x => new
            {
                x.Capsule,
                HoursAge = (now - x.CreationTime).TotalHours + 2,
                BasePoints = (x.LikeCount * 5) + (x.CommentCount * 10) + (x.IsLikedOtherUsers ? 50 : 0) + 1
            })
            .Select(x => new
            {
                x.Capsule,
                Score = x.BasePoints / Math.Pow(x.HoursAge, 1.5)
            })
            .OrderByDescending(x => x.Score)
            .Skip(capsuleFilters.SkipCount)
            .Take(capsuleFilters.MaxResultCount)
            .ToList();

        var items = capsulesWithScores.Select(x => x.Capsule).ToList();
        var creatorIds = items.Select(x => (Guid)x.CreatorId!).ToHashSet();
        var userProfiles = await UserProfileManager
            .TryGetListByQueryableAsync(q => q
                    .Include(u => u.User)
                    .Where(p => creatorIds.Contains(p.UserId)),
                cancellationToken: cancellationToken);

        var dtos = items.Select(x =>
        {
            var profile = userProfiles.FirstOrDefault(p => p.UserId == x.CreatorId);
            var fileUrl = LazyServiceProvider.GetDecryptedFileUrlAsync(x.CapsuleItems.FileUrl);
            return new CapsuleDto(
                x.Id,
                (Guid)x.CreatorId!,
                x.Name,
                CapsuleMapper.ResolveType(x.CapsuleType),
                profile?.User.UserName,
                LazyServiceProvider.GetDecryptedFileUrlAsync(profile?.ProfilePictureUrl),
                fileUrl,
                x.RevealDate,
                x.CreationTime
            );
        }).ToList();

        return new PagedResultDto<CapsuleDto>
        {
            Items = dtos,
            TotalCount = count
        };
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var capsule = await CapsuleManager.TryGetByAsync(x =>
                x.Id.Equals(id), true,
            cancellationToken: cancellationToken);

        await CapsuleRepository.DeleteAsync(capsule!, cancellationToken: cancellationToken);
        return true;
    }

    public async Task<bool> SetActivityAsync(
        Guid id,
        bool isActive = true,
        CancellationToken cancellationToken = default
    )
    {
        var capsule = await CapsuleManager.TryGetByAsync(x =>
                x.Id.Equals(id), true,
            cancellationToken: cancellationToken);
        capsule!.IsActive = isActive;
        await CapsuleRepository.UpdateAsync(capsule, cancellationToken: cancellationToken);
        return true;
    }

    private async Task<List<CapsuleLikeDto>> MapLikesAsync(List<CapsuleLike>? likes,
        Dictionary<Guid, UserProfile> profiles)
    {
        var dtos = new List<CapsuleLikeDto>();
        if (likes.IsNullOrEmpty()) return dtos;
        foreach (var like in likes)
        {
            var profile = profiles.GetValueOrDefault(like.UserId);
            dtos.Add(new CapsuleLikeDto
            {
                UserName = like.User.UserName,
                UserProfilePictureUrl = LazyServiceProvider.GetDecryptedFileUrlAsync(profile?.ProfilePictureUrl)
            });
        }

        return dtos;
    }

    private async Task<List<CapsuleCommentDto>> MapCommentsAsync(List<CapsuleComment>? comments,
        Dictionary<Guid, UserProfile> profiles)
    {
        var dtos = new List<CapsuleCommentDto>();
        if (comments.IsNullOrEmpty()) return dtos;
        foreach (var comment in comments)
        {
            var profile = profiles.GetValueOrDefault(comment.UserId);
            dtos.Add(new CapsuleCommentDto
            {
                UserName = comment.User.UserName,
                Comment = comment.Comment,
                UserProfilePictureUrl = LazyServiceProvider.GetDecryptedFileUrlAsync(profile?.ProfilePictureUrl)
            });
        }

        return dtos;
    }
}