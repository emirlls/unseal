using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unseal.Dtos.Messages;
using Unseal.Services.Messages;

namespace Unseal.Controllers.Messages;

[Authorize]
[ApiController]
[Route("messages")]
public class MessageController : UnsealController
{
    private IChatMessageService ChatMessageService =>
        LazyServiceProvider.LazyGetRequiredService<IChatMessageService>();

    /// <summary>
    /// Use send message to user or group by target ids in dto.
    /// </summary>
    /// <param name="chatMessageCreateDto"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost]
    public async Task SendMessageAsync(
        ChatMessageCreateDto chatMessageCreateDto,
        CancellationToken cancellationToken = default
    ) => await ChatMessageService.SendMessageAsync(chatMessageCreateDto, cancellationToken);
}