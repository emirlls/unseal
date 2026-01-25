using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Unseal.Etos;
using Unseal.Interfaces.Managers.Users;
using Unseal.Models.Users;
using Unseal.Repositories.Users;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Unseal.Handlers;

public class UserProfileEventHandler : IDistributedEventHandler<UserProfileEto>, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public UserProfileEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleEventAsync(UserProfileEto eventData)
    {
        var userProfileCreateModel = new UserProfileCreateModel(
            eventData.UserId,
            null,
            null,
            DateTime.Now
        );
        var userProfileManager = _serviceProvider.GetRequiredService<IUserProfileManager>();
        var userProfileRepository = _serviceProvider.GetRequiredService<IUserProfileRepository>();
        var userProfile = userProfileManager.CreateUserProfile(userProfileCreateModel);
        await userProfileRepository.InsertAsync(userProfile);
    }
}