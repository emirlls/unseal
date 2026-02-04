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
    public IResult GetCapsuleFeedStreamAsync(
        CancellationToken cancellationToken = default)
    {
        var stream = _serverSentEventAppService.GetCapsuleStreamAsync(cancellationToken);
        return TypedResults.ServerSentEvents(stream);
    }
    
    /// <summary>
    /// Use to send notification to user when follow request is accepted.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("accept-follow-request-stream")]
    [Authorize(CapsulePermissions.Default)]
    public IResult GetFollowRequestAcceptStreamAsync(
        CancellationToken cancellationToken = default)
    {
        var stream = _serverSentEventAppService.GetFollowRequestAcceptStreamAsync(cancellationToken);
        return TypedResults.ServerSentEvents(stream);
    }
}