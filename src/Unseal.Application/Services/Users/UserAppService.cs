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

        var userFollower = UserFollowerManager.Create((Guid)CurrentUser.Id!, user.Id);
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
}