using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Dtos.Groups;
using Unseal.Entities.Groups;
using Unseal.Extensions;
using Unseal.Filtering.Groups;
using Unseal.Interfaces.Managers.Groups;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Profiles.Groups;
using Unseal.Repositories.Groups;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Unseal.Services.Groups;

public class GroupAppService : UnsealAppService, IGroupAppService
{
    private IGroupManager GroupManager =>
        LazyServiceProvider.LazyGetRequiredService<IGroupManager>();

    private IGroupMemberManager GroupMemberManager =>
        LazyServiceProvider.LazyGetRequiredService<IGroupMemberManager>();

    private IGroupRepository GroupRepository =>
        LazyServiceProvider.LazyGetRequiredService<IGroupRepository>();

    private IGroupMemberRepository GroupMemberRepository =>
        LazyServiceProvider.LazyGetRequiredService<IGroupMemberRepository>();

    private GroupMapper GroupMapper =>
        LazyServiceProvider.LazyGetRequiredService<GroupMapper>();

    private IUserProfileManager UserProfileManager =>
        LazyServiceProvider.LazyGetRequiredService<IUserProfileManager>();

    private IStringLocalizer<UnsealResource> StringLocalizer =>
        LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<UnsealResource>>();

    [UnitOfWork]
    public async Task<bool> CreateGroupAsync(
        GroupCreateDto groupCreateDto,
        CancellationToken cancellationToken = default
    )
    {
        await CheckUserAllowToJoinGroupAsync(
            groupCreateDto.UserIds.ToHashSet(),
            cancellationToken
        );
        var groupCreateModel = GroupMapper.MapToModel(groupCreateDto);
        var group = GroupManager.Create(groupCreateModel);
        group = await SetGroupImageUrlAsync(
            group,
            groupCreateDto.StreamContent,
            cancellationToken);

        var groupMembers = GroupMemberManager.Create(
            groupCreateModel,
            group.Id,
            (Guid)CurrentUser.Id!
        );
        await GroupRepository.InsertAsync(group, cancellationToken: cancellationToken);
        await GroupMemberRepository.BulkInsertAsync(groupMembers, cancellationToken);
        return true;
    }

    public async Task<bool> UpdateGroupAsync(
        Guid groupId,
        GroupUpdateDto groupUpdateDto,
        CancellationToken cancellationToken = default
    )
    {
        // Only the group creator can update the group.
        var currentUserId = CurrentUser.GetId();
        var group = await GroupManager.TryGetByQueryableAsync(q =>
                q
                    .Where(x => x.Id.Equals(groupId) && x.CreatorId == currentUserId)
                    .Include(x => x.GroupMembers),
            throwIfNull: true,
            cancellationToken: cancellationToken);
        var existingMemberIds = group.GroupMembers
            .Select(x => x.UserId)
            .ToHashSet();
        await CheckUserAllowToJoinGroupAsync(
            groupUpdateDto.UserIds
                .Where(x => !existingMemberIds.Contains(x))
                .ToHashSet(),
            cancellationToken
        );
        var groupUpdateModel = GroupMapper.MapToModel(groupUpdateDto);
        var updatedGroup = GroupManager.Update(group, groupUpdateModel);
        updatedGroup = await SetGroupImageUrlAsync(
            updatedGroup,
            groupUpdateDto.StreamContent,
            cancellationToken);
        var groupMembers = GroupMemberManager.Create(
            groupUpdateModel,
            group.Id,
            (Guid)CurrentUser.Id!
        );
        await GroupRepository.UpdateAsync(updatedGroup, cancellationToken: cancellationToken);
        await GroupMemberRepository.HardDeleteManyAsync(group.GroupMembers, cancellationToken);
        await GroupMemberRepository.BulkInsertAsync(groupMembers, cancellationToken);
        return true;
    }

    private async Task<Group> SetGroupImageUrlAsync(
        Group group,
        IRemoteStreamContent? streamContent,
        CancellationToken cancellationToken = default)
    {
        if (streamContent is not null)
        {
            var fileUrl = await LazyServiceProvider
                .UploadFileAsync(streamContent);
            group.GroupImageUrl = LazyServiceProvider.GetEncryptedFileUrlAsync(fileUrl)!;
        }

        return group;
    }
    public async Task<PagedResultDto<GroupDto>> GetFilteredGroupListAsync(
        GroupFilters groupFilters,
        CancellationToken cancellationToken = default
    )
    {
        // Only the groups the current user belongs to are listed.
        var currentUserId = CurrentUser.GetId();
        Func<IQueryable<Group>, IQueryable<Group>> queryBuilder = q => q
            .Where(x => x.GroupMembers.Any(m => m.UserId == currentUserId));

        var groups = await GroupRepository.GetDynamicListAsync(
            groupFilters,
            queryBuilder,
            true,
            cancellationToken
        );

        var count = await GroupRepository.GetDynamicListCountAsync(
            groupFilters,
            queryBuilder,
            useCache:false,
            cancellationToken
        );

        var dto = GroupMapper.MapToDto(groups);
        var response = new PagedResultDto<GroupDto>
        {
            Items = dto,
            TotalCount = count
        };
        return response;
    }

    public async Task<GroupDetailDto> GetDetailAsync(
        Guid groupId,
        CancellationToken cancellationToken = default
    )
    {
        var currentUserId = CurrentUser.GetId();
        var group = await GroupManager.TryGetByQueryableAsync(q =>
                q
                    .Where(x => x.Id.Equals(groupId) &&
                                x.GroupMembers.Any(m => m.UserId == currentUserId))
                    .Include(x => x.GroupMembers)
                    .ThenInclude(x => x.User),
            throwIfNull: true,
            cancellationToken: cancellationToken);
        var dto = GroupMapper.MapToDetailDto(group);
        return dto;
    }

    public async Task<bool> LeaveAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await GroupManager.TryGetByQueryableAsync(q =>
                q
                    .Where(x => x.Id.Equals(groupId) &&
                                x.GroupMembers.Any(g => g.UserId.Equals(CurrentUser.Id)))
                    .Include(x => x.GroupMembers),
            throwIfNull: true,
            cancellationToken: cancellationToken);
        var groupMember = group.GroupMembers.FirstOrDefault(g => g.UserId.Equals(CurrentUser.Id));
        await GroupMemberRepository.HardDeleteAsync(groupMember!, cancellationToken);
        return true;
    }

    private async Task CheckUserAllowToJoinGroupAsync(
        HashSet<Guid> userIds,
        CancellationToken cancellationToken
    )
    {
        var userAllowJoinGroups = (await UserProfileManager
                .TryGetListByAsync(x =>
                        userIds.Contains(x.UserId),
                    cancellationToken: cancellationToken))
            .ToDictionary(x => x.UserId, x => x.AllowJoinGroup);

        await Parallel.ForEachAsync(
            userIds,
            cancellationToken,
            async (userId, ct) =>
            {
                if (userAllowJoinGroups.TryGetValue(userId, out var allowJoinGroup))
                {
                    if (!allowJoinGroup)
                    {
                        throw new UserFriendlyException(
                            StringLocalizer[ExceptionCodes.GroupMember.UserNotAllowedToJoinGroup]);
                    }
                }
            });
    }
}