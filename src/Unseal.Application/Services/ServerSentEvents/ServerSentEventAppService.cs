using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Unseal.Constants;
using Unseal.Enums;
using Unseal.Extensions;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Interfaces.Managers.Users;
using Unseal.Models.ServerSentEvents;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Unseal.Services.ServerSentEvents;

public class ServerSentEventAppService : UnsealAppService, IServerSentEventAppService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ICapsuleManager _capsuleManager;
    private readonly IUserViewTrackingManager _userViewTrackingManager;
    private readonly IUserProfileManager _userProfileManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ServerSentEventAppService(
        IConnectionMultiplexer connectionMultiplexer,
        IUserViewTrackingManager userViewTrackingManager,
        ICapsuleManager capsuleManager,
        IUserProfileManager userProfileManager,
        IUnitOfWorkManager unitOfWorkManager
    )
    {
        _connectionMultiplexer = connectionMultiplexer;
        _userViewTrackingManager = userViewTrackingManager;
        _capsuleManager = capsuleManager;
        _userProfileManager = userProfileManager;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async IAsyncEnumerable<SseItem<CapsuleCreatedEventModel>> GetCapsuleStreamAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var subscriber = _connectionMultiplexer.GetSubscriber();
        var channel = Channel.CreateUnbounded<string>();
        var currentUserId = CurrentUser.GetId();
        await subscriber.SubscribeAsync(
            EventConstants.ServerSentEvents.CapsuleCreate.GlobalFeedUpdateChannel,
            (redisChannel, message) => channel.Writer.TryWrite(message.ToString())
        );

        try
        {
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                var trackingQueryable = await _userViewTrackingManager
                    .TryGetQueryableAsync(q => q
                            .Where(v => v.UserId == currentUserId)
                            .OrderByDescending(v => v.CreationTime),
                        cancellationToken: cancellationToken
                    );

                DateTime referenceDate;

                if (trackingQueryable != null)
                {
                    var lastView = await trackingQueryable.FirstOrDefaultAsync(cancellationToken);
                    referenceDate = lastView?.CreationTime ?? DateTime.UtcNow.AddDays(-1);
                }
                else
                {
                    referenceDate = DateTime.UtcNow.AddDays(-1);
                }

                var missedCapsules = await _capsuleManager.TryGetListByQueryableAsync(q => q
                        .Include(x => x.CapsuleItems)
                        .Where(x => x.CreationTime > referenceDate && x.CapsuleType.Code == (int)CapsuleTypes.Public &&
                                    (bool)x.IsOpened!),
                    cancellationToken: cancellationToken);

                if (!missedCapsules.IsNullOrEmpty())
                {
                    var creatorIds = missedCapsules
                        .Select(x => (Guid)x.CreatorId!)
                        .ToHashSet();

                    var profiles = await _userProfileManager.TryGetListByQueryableAsync(q => q
                            .Include(x => x.User)
                            .Where(x => creatorIds.Contains(x.UserId)),
                        cancellationToken: cancellationToken
                    );

                    foreach (var capsule in missedCapsules)
                    {
                        var profile = profiles.FirstOrDefault(p => p.UserId == capsule.CreatorId);
                        var decryptedFileUrl =
                            LazyServiceProvider.GetDecryptedFileUrlAsync(capsule.CapsuleItems.FileUrl);
                        var decryptedProfilePictureUrl =
                            LazyServiceProvider.GetDecryptedFileUrlAsync(profile?.ProfilePictureUrl);
                        var missedEvent = new CapsuleCreatedEventModel
                        {
                            Id = capsule.Id,
                            CreatorId = (Guid)capsule.CreatorId!,
                            Name = capsule.Name,
                            Username = profile?.User.UserName,
                            FileUrl = decryptedFileUrl,
                            ProfilePictureUrl = decryptedProfilePictureUrl,
                            RevealDate = capsule.RevealDate,
                            CreationTime = capsule.CreationTime
                        };

                        yield return new SseItem<CapsuleCreatedEventModel>(
                            missedEvent,
                            EventConstants.ServerSentEvents.CapsuleCreate.Type)
                        {
                            EventId = capsule.CreationTime.ToString("O")
                        };
                    }

                    await uow.CompleteAsync(cancellationToken);
                }
            }

            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                var eventData = JsonSerializer.Deserialize<CapsuleCreatedEventModel>(message);
                yield return new SseItem<CapsuleCreatedEventModel>(eventData!,
                    EventConstants.ServerSentEvents.CapsuleCreate.Type)
                {
                    EventId = DateTimeOffset.UtcNow.ToString("O"),
                    ReconnectionInterval = TimeSpan.FromSeconds(5)
                };
            }
        }
        finally
        {
            channel.Writer.TryComplete();
            await subscriber
                .UnsubscribeAsync(EventConstants.ServerSentEvents.CapsuleCreate.GlobalFeedUpdateChannel);
        }
    }

    public async IAsyncEnumerable<SseItem<FollowRequestAcceptedEventModel>> GetFollowRequestAcceptStreamAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var subscriber = _connectionMultiplexer.GetSubscriber();
        var channel = Channel.CreateUnbounded<string>();
        var currentUserId = CurrentUser.GetId();
        await subscriber.SubscribeAsync(
            EventConstants.ServerSentEvents.FollowRequestAccept.FollowRequestAcceptChannel,
            (redisChannel, message) => channel.Writer.TryWrite(message.ToString())
        );

        try
        {
            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                var eventData = JsonSerializer.Deserialize<FollowRequestAcceptedEventModel>(message);
                if(eventData.FollowerUserId != currentUserId) continue;
                yield return new SseItem<FollowRequestAcceptedEventModel>(eventData!,
                    EventConstants.ServerSentEvents.FollowRequestAccept.Type)
                {
                    EventId = DateTimeOffset.UtcNow.ToString("O"),
                    ReconnectionInterval = TimeSpan.FromSeconds(5)
                };
            }
        }
        finally
        {
            channel.Writer.TryComplete();
            await subscriber
                .UnsubscribeAsync(EventConstants.ServerSentEvents.FollowRequestAccept.FollowRequestAcceptChannel);
        }
    }
}