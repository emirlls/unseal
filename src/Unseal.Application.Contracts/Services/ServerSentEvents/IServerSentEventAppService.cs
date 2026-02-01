using System.Collections.Generic;
using System.Net.ServerSentEvents;
using System.Threading;
using Unseal.Models.ServerSentEvents;
using Volo.Abp.Application.Services;

namespace Unseal.Services.ServerSentEvents;

public interface IServerSentEventAppService : IApplicationService
{
    IAsyncEnumerable<SseItem<CapsuleCreatedEventModel>> GetCapsuleStreamAsync(
        CancellationToken cancellationToken = default
    );
}