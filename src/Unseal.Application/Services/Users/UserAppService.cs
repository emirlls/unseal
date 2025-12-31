using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Interfaces.Managers.Auth;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Repositories.Users;
using Volo.Abp;

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
    private IUserInteractionRepository UserInteractionRepository =>
        LazyServiceProvider.LazyGetRequiredService<IUserInteractionRepository>();
    private IStringLocalizer<UnsealResource> StringLocalizer =>
        LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<UnsealResource>>();
    public async Task<bool> FollowAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await CustomIdentityUserManager
            .TryGetByAsync(x => 
                    x.Id.Equals(userId), throwIfNull: true,
            cancellationToken: cancellationToken);
        var isBlocked = await UserInteractionManager.CheckUserBlockedAsync(user.Id, (Guid)CurrentUser.Id!, cancellationToken);
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

    public async Task<bool> BlockAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userInteraction = await UserInteractionManager.TryGetByAsync(x =>
            x.SourceUserId.Equals(CurrentUser.Id) && x.TargetUserId.Equals(userId), cancellationToken: cancellationToken);
        if (userInteraction is null)
        {
            var entity = UserInteractionManager.Create((Guid)CurrentUser.Id!, userId);
            entity.IsBlocked = true;
            await UserInteractionRepository.InsertAsync(entity, cancellationToken: cancellationToken);
            return true;
        }

        userInteraction.IsBlocked = true;
        await UserInteractionRepository.UpdateAsync(userInteraction, cancellationToken: cancellationToken);
        return true;
    }

    public async Task<bool> UnBlockAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userInteraction = await UserInteractionManager.TryGetByAsync(x =>
            x.SourceUserId.Equals(CurrentUser.Id) && x.TargetUserId.Equals(userId), cancellationToken: cancellationToken);
        
        if (userInteraction is null)
        {
            var entity = UserInteractionManager.Create((Guid)CurrentUser.Id!, userId);
            await UserInteractionRepository.InsertAsync(entity, cancellationToken: cancellationToken);
            return true;
        }

        userInteraction.IsBlocked = false;
        await UserInteractionRepository.UpdateAsync(userInteraction, cancellationToken: cancellationToken);
        return true;
    }
}