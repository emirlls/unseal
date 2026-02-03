using System.Threading;
using System.Threading.Tasks;
using Unseal.Dtos.Messages;
using Volo.Abp.Application.Services;

namespace Unseal.Services.Messages;

public interface IChatMessageService : IApplicationService
{
    Task SendMessageAsync(
        ChatMessageCreateDto chatMessageCreateDto,
        CancellationToken cancellationToken = default
    );
}