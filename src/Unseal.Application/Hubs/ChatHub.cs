using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Unseal.Models.Hubs;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace Unseal.Hubs;

[Authorize]
public class ChatHub : AbpHub
{
    private static readonly ConcurrentDictionary<string, ChatHubConnectionModel> Connections = new();
    
    public override Task OnConnectedAsync()
    {
        var userId = CurrentUser.GetId();
        var connectionId = Context.ConnectionId;
        Connections.TryAdd(connectionId, new ChatHubConnectionModel
        {
            UserId = userId,
            ConnectionId = Context.ConnectionId,
            CultureKey = Context.GetHttpContext()!.Request.Query["cultureKey"].ToString()
        });
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Connections.TryRemove(Context.ConnectionId, out _);
        return base.OnDisconnectedAsync(exception);
    }
    [HubMethodName("join-group")]
    public async Task JoinGroup(Guid groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
    }

    [HubMethodName("leave-group")]
    public async Task LeaveGroup(Guid groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
    }
}