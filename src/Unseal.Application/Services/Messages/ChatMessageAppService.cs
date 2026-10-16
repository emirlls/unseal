using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Dtos.Messages;
using Unseal.Enums;
using Unseal.Hubs;
using Unseal.Interfaces.Managers.Groups;
using Unseal.Interfaces.Managers.Messages;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Profiles.Messages;
using Unseal.Repositories.Messages;
using Volo.Abp;
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

    private IGroupMemberManager GroupMemberManager =>
        LazyServiceProvider.LazyGetRequiredService<IGroupMemberManager>();

    private IUserInteractionManager UserInteractionManager =>
        LazyServiceProvider.LazyGetRequiredService<IUserInteractionManager>();

    private IStringLocalizer<UnsealResource> StringLocalizer =>
        LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<UnsealResource>>();

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

        var currentUserId = CurrentUser.GetId();
        await CheckCanSendMessageAsync(
            chatType!.Code,
            chatMessageCreateDto.TargetIds.ToHashSet(),
            currentUserId,
            cancellationToken
        );

        var chatMessageCreateModel = ChatMessageMapper
            .MapToModel(chatMessageCreateDto, currentUserId);
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
                    .SendAsync(
                        HubConstants.Methods.ReceiveMessage,
                        chatMessageCreateModel.Content,
                        cancellationToken
                    );
                await _hubContext.Clients
                    .User(chatMessageCreateModel.SenderId.ToString())
                    .SendAsync(
                        HubConstants.Methods.ReceiveMessage,
                        chatMessageCreateModel.Content,
                        cancellationToken
                    );
                break;

            case (int)ChatTypes.Group:
                targetIds.Add(chatMessageCreateModel.SenderId.ToString());
                await _hubContext.Clients
                    .Groups(targetIds)
                    .SendAsync(
                        HubConstants.Methods.ReceiveMessage,
                        chatMessageCreateModel.Content, 
                        cancellationToken
                    );
                break;
        }
    }

    private async Task CheckCanSendMessageAsync(
        int chatTypeCode,
        HashSet<Guid> targetIds,
        Guid senderId,
        CancellationToken cancellationToken = default
    )
    {
        switch (chatTypeCode)
        {
            case (int)ChatTypes.Directly:
                // Direct messages are not allowed when either side has blocked the other.
                var isBlocked = await UserInteractionManager.ExistsAsync(x =>
                        x.IsBlocked &&
                        ((x.SourceUserId == senderId && targetIds.Contains(x.TargetUserId)) ||
                         (x.TargetUserId == senderId && targetIds.Contains(x.SourceUserId))),
                    cancellationToken: cancellationToken);
                if (isBlocked)
                {
                    throw new UserFriendlyException(StringLocalizer[ExceptionCodes.ChatMessage.UserIsBlocked]);
                }

                break;

            case (int)ChatTypes.Group:
                // Group messages can only be sent to groups the sender belongs to.
                var memberGroupIds = (await GroupMemberManager.TryGetListByAsync(x =>
                            targetIds.Contains(x.GroupId) && x.UserId == senderId,
                        asNoTracking: true,
                        cancellationToken: cancellationToken))
                    .Select(x => x.GroupId)
                    .ToHashSet();
                if (!targetIds.IsSubsetOf(memberGroupIds))
                {
                    throw new UserFriendlyException(StringLocalizer[ExceptionCodes.GroupMember.UserIsNotMember]);
                }

                break;
        }
    }
}