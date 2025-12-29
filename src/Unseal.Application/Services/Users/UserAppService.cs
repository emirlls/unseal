using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Dtos.Users;
using Unseal.Extensions;
using Unseal.Interfaces.Managers.Auth;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Profiles.Users;
using Unseal.Repositories.Users;
using Volo.Abp;
using Volo.Abp.Uow;

namespace Unseal.Services.Users;

public class UserAppService : UnsealAppService, IUserAppService
{
    private ICustomIdentityUserManager CustomIdentityUserManager =>
        LazyServiceProvider.LazyGetRequiredService<ICustomIdentityUserManager>();
    private IUserFollowerManager UserFollowerManager =>
        LazyServiceProvider.LazyGetRequiredService<IUserFollowerManager>();
    private IUserFollowerRepository UserFollowerRepository =>
        LazyServiceProvider.LazyGetRequiredService<IUserFollowerRepository>();
    private IUserProfileManager UserProfileManager =>
        LazyServiceProvider.LazyGetRequiredService<IUserProfileManager>();
    private UserMapper UserMapper =>
        LazyServiceProvider.LazyGetRequiredService<UserMapper>();
    private IGroupManager GroupManager =>
        LazyServiceProvider.LazyGetRequiredService<IGroupManager>();
    private IGroupMemberManager GroupMemberManager =>
        LazyServiceProvider.LazyGetRequiredService<IGroupMemberManager>();
    private IGroupRepository GroupRepository =>
        LazyServiceProvider.LazyGetRequiredService<IGroupRepository>();
    private IGroupMemberRepository GroupMemberRepository =>
        LazyServiceProvider.LazyGetRequiredService<IGroupMemberRepository>();
    private IStringLocalizer<UnsealResource> StringLocalizer =>
        LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<UnsealResource>>();
    public async Task<bool> FollowAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await CustomIdentityUserManager
            .TryGetByAsync(x => 
                    x.Id.Equals(userId), throwIfNull: true,
            cancellationToken: cancellationToken);
        var isBlocked = await UserFollowerManager.CheckUserBlockedAsync(CurrentUser.Id, user.Id, cancellationToken);
        if (isBlocked)
        {
            throw new UserFriendlyException(StringLocalizer[ExceptionCodes.UserFollower.UserIsBanned]);
        }

        var userFollower = UserFollowerManager.Create(user.Id, (Guid)CurrentUser.Id!);
        await UserFollowerRepository.InsertAsync(userFollower, cancellationToken: cancellationToken);
        return true;
    }

    public async Task<bool> UnfollowAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await CustomIdentityUserManager
            .TryGetByAsync(x => 
                    x.Id.Equals(userId), throwIfNull: true,
                cancellationToken: cancellationToken);

        var userFollower = await UserFollowerManager.TryGetByAsync(x =>
            x.UserId.Equals(user.Id) && 
            x.FollowerId.Equals((Guid)CurrentUser.Id!),
            cancellationToken: cancellationToken
        );
        await UserFollowerRepository.HardDeleteAsync(userFollower, cancellationToken);
        return true;
    }

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
        var groupCreateModel = UserMapper.MapGroupCreateDtoToModel(groupCreateDto);
        var group = GroupManager.Create(groupCreateModel);
        if (groupCreateDto.StreamContent is not null)
        {
            var fileUrl = await LazyServiceProvider
                .UploadFileAsync(groupCreateDto.StreamContent);
            group.GroupImageUrl = fileUrl;
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

        var group = await GroupManager.TryGetQueryableAsync(q=>
            q
                .Where(x=>x.Id.Equals(groupId))
                .Include(x=>x.GroupMembers),
            cancellationToken: cancellationToken);
        
        var groupUpdateModel = UserMapper.MapGroupUpdateDtoToModel(groupUpdateDto);
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

    private async Task CheckUserAllowToJoinGroupAsync(
        HashSet<Guid> userIds,
        CancellationToken cancellationToken
    )
    {
        var userAllowJoinGroups = (await UserProfileManager
                .TryGetListByAsync(x => 
                        userIds.Contains(x.UserId), 
                    cancellationToken: cancellationToken))
            .ToDictionary(x=>x.UserId,x=>x.AllowJoinGroup);

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