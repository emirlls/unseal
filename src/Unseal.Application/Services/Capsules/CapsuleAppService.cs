using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Unseal.Constants;
using Unseal.Dtos.Capsules;
using Unseal.Entities.Capsules;
using Unseal.Extensions;
using Unseal.Filtering.Capsules;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Interfaces.Managers.Users;
using Unseal.Profiles.Capsules;
using Unseal.Repositories.Capsules;
using Volo.Abp.Application.Dtos;

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
    public async Task<bool> CreateAsync(CapsuleCreateDto capsuleCreateDto,
        CancellationToken cancellationToken = default)
    {
        var fileUrl = await LazyServiceProvider.UploadFileAsync(capsuleCreateDto.StreamContent);
        var encryptedFileUrl = LazyServiceProvider.GetEncryptedFileUrlAsync(fileUrl);
        var capsuleCreateModel = CapsuleMapper.MaptoModel(capsuleCreateDto);
        var capsule = CapsuleManager.Create(capsuleCreateModel, CurrentUser.Id!);
        capsuleCreateModel = capsuleCreateModel with { FileUrl = encryptedFileUrl };
        var capsuleItem = CapsuleItemManager.Create(capsuleCreateModel, capsule.Id);
        await CapsuleRepository.InsertAsync(capsule, cancellationToken: cancellationToken);
        await CapsuleItemRepository.InsertAsync(capsuleItem, cancellationToken: cancellationToken);
        await CreateCapsuleMapFeatureAsync(capsule.Id, capsuleCreateModel.GeoJson, cancellationToken);
        var redis = LazyServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        var subscriber = redis.GetSubscriber();

        var payload = JsonSerializer.Serialize(new
        {
            type = EventConstants.ServerSentEvents.CapsuleCreate.Type,
            capsuleId = capsule.Id,
            creationTime = DateTime.Now
        });

        await subscriber.PublishAsync(
            EventConstants.ServerSentEvents.CapsuleCreate.GlobalFeedUpdates,
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
        capsuleFilters.SetIsOpened(true);
        capsuleFilters.SetIsPublic(Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Public.Id));
        Func<IQueryable<Capsule>, IQueryable<Capsule>>? queryBuilder = null;
        if (!isAll)
        {
            queryBuilder = capsule => capsule
                .Include(x => x.CapsuleType)
                .Where(c => c.CreatorId == CurrentUser.Id);
        }
        else
        {
            var usersBlockedCurrentUser = await UserInteractionManager
                .TryGetQueryableAsync(x =>
                        x.Where(c =>
                            c.TargetUserId.Equals(CurrentUser.Id) && c.IsBlocked),
                    cancellationToken: cancellationToken);
            var capsuleCreators =
                usersBlockedCurrentUser != null && usersBlockedCurrentUser.Any()
                    ? usersBlockedCurrentUser
                        .Select(x => x.SourceUserId)
                        .ToHashSet()
                    : null;
            queryBuilder = capsule => capsule
                .Include(x => x.CapsuleType)
                .WhereIf(!capsuleCreators.IsNullOrEmpty(),
                    x => !capsuleCreators.Contains((Guid)x.CreatorId!));
        }

        var capsules = await CapsuleRepository
            .GetDynamicListAsync(capsuleFilters, queryBuilder, true, cancellationToken);
        var count = await CapsuleRepository
            .GetDynamicListCountAsync(
                capsuleFilters, 
                useCache:false,
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
            var userProfile =  userProfiles.FirstOrDefault(u=>u.UserId.Equals(x.CreatorId));
            return new CapsuleDto(
                x.Id,
                (Guid)x.CreatorId!,
                x.Name,
                capsuleType,
                userProfile.User.UserName,
                userProfile.ProfilePictureUrl,
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
        await CapsuleManager.TryGetByAsync(x =>
                x.Id.Equals(capsuleId), true,
            cancellationToken: cancellationToken);
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
}