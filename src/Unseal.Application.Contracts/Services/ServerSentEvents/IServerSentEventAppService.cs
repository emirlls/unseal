using System.Collections.Generic;
using System.Net.ServerSentEvents;
using System.Threading;
using Volo.Abp.Application.Services;

namespace Unseal.Services.ServerSentEvents;

public interface IServerSentEventAppService : IApplicationService
{
    IAsyncEnumerable<SseItem<object>> GetCapsuleStreamAsync(
        string? lastId,
        CancellationToken cancellationToken = default);
}