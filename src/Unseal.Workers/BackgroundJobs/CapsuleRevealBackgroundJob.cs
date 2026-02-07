using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Unseal.Constants;
using Unseal.Extensions;
using Unseal.Filtering.Base;
using Unseal.Filtering.Capsules;
using Unseal.Interfaces.Managers.Users;
using Unseal.Models.ServerSentEvents;
using Unseal.Repositories.Capsules;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Unseal.Workers.BackgroundJobs;

public class CapsuleRevealBackgroundJob : ITransientDependency
{
    public async Task RevealCapsulesAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        var uowManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        using (var uow = uowManager.Begin(new AbpUnitOfWorkOptions { IsTransactional = true }))
        {
            var tenantDataFilter = serviceProvider.GetRequiredService<IDataFilter<IMultiTenant>>();
            using (tenantDataFilter.Disable())
            {
                var capsuleRepository = serviceProvider.GetRequiredService<ICapsuleRepository>();
                var now = DateTime.Now;
                var capsuleFilters = new CapsuleFilters
                {
                    Filters = new List<FilterItem>()
                    {
                        new FilterItem
                        {
                            Prop = CapsuleFilters.GetIsOpened(),
                            Strategy = FilterOperators.Equals,
                            Value = false.ToString()
                        },
                        new FilterItem
                        {
                            Prop = CapsuleFilters.GetRevealDate(),
                            Strategy = FilterOperators.LessThanOrEqual,
                            Value = now.AddMinutes(-5).ToString(CultureInfo.InvariantCulture)
                        }
                    }
                };
                var notOpenedCapsules = await capsuleRepository
                    .GetDynamicListAsync(
                        capsuleFilters,
                        x => x
                            .Include(c => c.CapsuleItems),
                        false,
                        cancellationToken
                    );
                if (notOpenedCapsules.IsNullOrEmpty()) return;
                notOpenedCapsules.ForEach(x => x.IsOpened = true);
                await capsuleRepository.BulkUpdateAsync(notOpenedCapsules, cancellationToken);

                var redis = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
                var userProfileManager = serviceProvider.GetRequiredService<IUserProfileManager>();
                var subscriber = redis.GetSubscriber();
                var creatorIds = notOpenedCapsules
                    .Select(x => x.CreatorId)
                    .ToHashSet();
                var userProfiles = await ((await userProfileManager
                        .TryGetQueryableAsync(x => x
                                .Include(c => c.User)
                                .Where(c => creatorIds.Contains(c.UserId)),
                            asNoTracking: true,
                            cancellationToken: cancellationToken
                        ))!)
                    .ToListAsync(cancellationToken: cancellationToken);
                foreach (var capsule in notOpenedCapsules)
                {
                    var userProfile = userProfiles!.FirstOrDefault(x => x.UserId.Equals(capsule.CreatorId));
                    var decryptedProfilePictureUrl =
                        serviceProvider.GetDecryptedFileUrlAsync(userProfile?.ProfilePictureUrl);
                    var fileUrl = serviceProvider.GetDecryptedFileUrlAsync(capsule.CapsuleItems.FileUrl);
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
                }
            }

            await uow.CompleteAsync(cancellationToken);
        }
    }
}