using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unseal.Permissions.Capsules;
using Unseal.Services.ServerSentEvents;

namespace Unseal.Controllers.ServerSentEvents;

[Authorize]
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
    /// Use to refresh capsule feed stream.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("capsule-feed-stream")]
    [Authorize(CapsulePermissions.Default)]
    public IResult GetCapsuleFeedStream(
        CancellationToken cancellationToken = default)
    {
        var stream = _serverSentEventAppService.GetCapsuleStreamAsync(cancellationToken);
        return TypedResults.ServerSentEvents(stream);
    }
}