using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Unseal.Constants;
using Unseal.Enums;
using Unseal.Etos;
using Unseal.Models.ElasticSearch;
using Unseal.Repositories.Users;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Unseal.Handlers;

public class UserElasticEventHandler : IDistributedEventHandler<UserElasticEto>, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public UserElasticEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleEventAsync(UserElasticEto eventData)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var userElasticRepository = scope.ServiceProvider.GetRequiredService<IEsUserRepository>();

            switch (eventData.UserElasticQueryTpe)
            {
                case (int)UserElasticQueryTypes.Create:
                    var userElasticCreateModel = new UserElasticModel
                    {
                        Id = eventData.UserId,
                        UserName = eventData.UserName,
                        Name = eventData.Name,
                        Surname = eventData.Surname,
                        ProfilePictureUrl = eventData.ProfilePictureUrl,
                        BlockedByUserIds = eventData.BlockedUserId.HasValue
                            ? new List<Guid> { eventData.BlockedUserId.Value }
                            : new List<Guid>()
                    };
                    await userElasticRepository.CreateDocumentAsync(userElasticCreateModel,
                        ElasticSearchConstants.User.UserSearchIndex);
                    break;

                case (int)UserElasticQueryTypes.Update:
                    if (eventData.BlockedUserId.HasValue)
                    {
                        await userElasticRepository.AddBlockedUserAsync(eventData.UserId,
                            eventData.BlockedUserId.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(eventData.ProfilePictureUrl))
                    {
                        await userElasticRepository.UpdateProfilePictureAsync(eventData.UserId,
                            eventData.ProfilePictureUrl);
                    }
                    break;
            }
        }
    }
}