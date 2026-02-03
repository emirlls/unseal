using System;

namespace Unseal.Models.Hubs;

public class ChatHubConnectionModel
{
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; }
    public string CultureKey { get; set; }
}