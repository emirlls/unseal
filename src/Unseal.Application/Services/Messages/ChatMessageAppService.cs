using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Unseal.Constants;
using Unseal.Dtos.Messages;
using Unseal.Enums;
using Unseal.Hubs;
using Unseal.Interfaces.Managers.Messages;
using Unseal.Profiles.Messages;
using Unseal.Repositories.Messages;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;

namespace Unseal.Services.Messages;

public class ChatMessageAppService : ApplicationService, IChatMessageService
{
    private IChatMessageManager ChatMessageManager =>
        LazyServiceProvider.LazyGetRequiredService<IChatMessageManager>();

    private IChatTypeManager ChatTypeManager =>
        LazyServiceProvider.LazyGetRequiredService<IChatTypeManager>();

    private IChatMessageRepository ChatMessageRepository =>
        LazyServiceProvider.LazyGetRequiredService<IChatMessageRepository>();

    private ChatMessageMapper ChatMessageMapper =>
        LazyServiceProvider.LazyGetRequiredService<ChatMessageMapper>();
    
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatMessageAppService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageAsync(
        ChatMessageCreateDto chatMessageCreateDto,
        CancellationToken cancellationToken = default
    )
    {
        var chatType = await ChatTypeManager
            .TryGetByAsync(x => x.Id.Equals(chatMessageCreateDto.ChatTypeId),
                true,
                cancellationToken: cancellationToken);

        var chatMessageCreateModel = ChatMessageMapper
            .MapToModel(chatMessageCreateDto, CurrentUser.GetId());
        var chatMessage = ChatMessageManager.Create(chatMessageCreateModel);
        await ChatMessageRepository.BulkInsertAsync(chatMessage, cancellationToken: cancellationToken);
        var targetIds = chatMessageCreateModel.TargetIds
            .Select(x=>x.ToString())
            .ToList();
        switch (chatType.Code)
        {
            case (int)ChatTypes.Directly:
                await _hubContext.Clients
                    .Users(targetIds)
                    .SendAsync(HubConstants.Methods.ReceiveMessage, chatMessageCreateModel.Content,
                        cancellationToken);
                await _hubContext.Clients
                    .User(chatMessageCreateModel.SenderId.ToString())
                    .SendAsync(HubConstants.Methods.ReceiveMessage, chatMessageCreateModel.Content,
                        cancellationToken);
                break;

            case (int)ChatTypes.Group:
                await _hubContext.Clients
                    .Groups(targetIds)
                    .SendAsync(HubConstants.Methods.ReceiveMessage, chatMessageCreateModel.Content, cancellationToken);
                break;
        }
    }
}