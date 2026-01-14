using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unseal.Constants;
using Unseal.Services.ServerSentEvents;

namespace Unseal.Controllers.ServerSentEvents;

[ApiController]
[Route("api/server-sent-events")]
public class ServerSentEventController : UnsealController
{
    private readonly IServerSentEventAppService _serverSentEventAppService;

    public ServerSentEventController(IServerSentEventAppService serverSentEventAppService)
    {
        _serverSentEventAppService = serverSentEventAppService;
    }

    /// <summary>
    /// USe to refresh capsule feed stream.
    /// </summary>
    /// <param name="access_token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("capsule-feed-stream")]
    [AllowAnonymous]
    public IResult GetCapsuleFeedStream(
        CancellationToken cancellationToken = default)
    {
        if (!Request.Headers.TryGetValue(EventConstants.ServerSentEvents.CapsuleCreate.LastEventId,
                out var lastEventId))
        {
            lastEventId = string.Empty;
        }

        var stream = _serverSentEventAppService.GetCapsuleStreamAsync(lastEventId, cancellationToken);
        return TypedResults.ServerSentEvents(stream);
    }
}