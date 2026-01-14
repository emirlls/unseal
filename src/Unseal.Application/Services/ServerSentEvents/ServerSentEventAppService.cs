using System;
using System.Collections.Generic;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using StackExchange.Redis;
using Unseal.Constants;
using Unseal.Repositories.Capsules;

namespace Unseal.Services.ServerSentEvents;

public class ServerSentEventAppService : UnsealAppService, IServerSentEventAppService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ICapsuleRepository _capsuleRepository;

    public ServerSentEventAppService(
        IConnectionMultiplexer connectionMultiplexer,
        ICapsuleRepository capsuleRepository
    )
    {
        _connectionMultiplexer = connectionMultiplexer;
        _capsuleRepository = capsuleRepository;
    }

    public async IAsyncEnumerable<SseItem<object>> GetCapsuleStreamAsync(string? lastId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var subscriber = _connectionMultiplexer.GetSubscriber();
        var channel = Channel.CreateUnbounded<string>();

        await subscriber.SubscribeAsync(
            EventConstants.ServerSentEvents.CapsuleCreate.GlobalFeedUpdates,
            (redisChannel, message) => channel.Writer.TryWrite(message.ToString())
        );

        try
        {
            if (!string.IsNullOrEmpty(lastId) && DateTimeOffset.TryParse(lastId, out var lastSeenDate))
            {
                var minDate = DateTime.UtcNow.AddDays(-1);
                var searchDate = lastSeenDate.UtcDateTime < minDate ? minDate : lastSeenDate.UtcDateTime;

                var missedCapsules = await _capsuleRepository.GetListByAsync(
                    x => x.CreationTime > searchDate,
                    cancellationToken: cancellationToken
                );

                foreach (var capsule in missedCapsules)
                {
                    yield return new SseItem<object>(
                        new
                        {
                            type = EventConstants.ServerSentEvents.CapsuleCreate.Type,
                            capsuleId = capsule.Id,
                            creationTime = capsule.CreationTime
                        },
                        EventConstants.ServerSentEvents.CapsuleCreate.Type)
                    {
                        EventId = capsule.CreationTime.ToString("O")
                    };
                }
            }

            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                var data = JsonSerializer.Deserialize<object>(message);
                var eventId = DateTimeOffset.UtcNow.ToString("O");
                yield return new SseItem<object>(data!, EventConstants.ServerSentEvents.CapsuleCreate.Type)
                {
                    EventId = eventId,
                    ReconnectionInterval = TimeSpan.FromSeconds(5)
                };
            }
        }
        finally
        {
            channel.Writer.TryComplete();
            await subscriber.UnsubscribeAsync(EventConstants.ServerSentEvents.CapsuleCreate.GlobalFeedUpdates);
        }
    }
}