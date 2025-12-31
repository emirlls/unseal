using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Dtos.Groups;
using Unseal.Extensions;
using Unseal.Filtering.Users;
using Unseal.Interfaces.Managers.Groups;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Profiles.Groups;
using Unseal.Repositories.Groups;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

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
        var groupCreateModel = GroupMapper.MapGroupCreateDtoToModel(groupCreateDto);
        var group = GroupManager.Create(groupCreateModel);
        if (groupCreateDto.StreamContent is not null)
        {
            var fileUrl = await LazyServiceProvider
                .UploadFileAsync(groupCreateDto.StreamContent);
            group.GroupImageUrl = LazyServiceProvider.GetEncryptedFileUrlAsync(fileUrl)!;
        }

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
        var group = await GroupManager.TryGetQueryableAsync(q =>
                q
                    .Where(x => x.Id.Equals(groupId))
                    .Include(x => x.GroupMembers),
            throwIfNull: true,
            cancellationToken: cancellationToken);

        var groupUpdateModel = GroupMapper.MapGroupUpdateDtoToModel(groupUpdateDto);
        var updatedGroup = GroupManager.Update(group, groupUpdateModel);
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

    public async Task<PagedResultDto<GroupDto>> GetFilteredGroupListAsync(
        GroupFilters groupFilters,
        CancellationToken cancellationToken = default
    )
    {
        var groups = await GroupRepository.GetDynamicListAsync(
            groupFilters,
            cancellationToken
        );

        var count = await GroupRepository.GetDynamicListCountAsync(
            groupFilters,
            cancellationToken
        );

        var dto = GroupMapper.MapGroupToGroupDtoList(groups);
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
        var group = await GroupManager.TryGetQueryableAsync(q =>
                q
                    .Where(x => x.Id.Equals(groupId))
                    .Include(x => x.GroupMembers)
                    .ThenInclude(x => x.User),
            throwIfNull: true,
            cancellationToken: cancellationToken);
        var dto = GroupMapper.MapGroupToGroupDetailDto(group);
        return dto;
    }

    public async Task<bool> LeaveAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await GroupManager.TryGetQueryableAsync(q =>
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