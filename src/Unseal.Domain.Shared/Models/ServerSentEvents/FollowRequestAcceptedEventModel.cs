using System;

namespace Unseal.Models.ServerSentEvents;

public class FollowRequestAcceptedEventModel
{
    public Guid FollowerUserId { get; set; }
    public Guid UserId { get; set; }
    public string UserName  { get; set; }
    public DateTime CreationTime { get; set; }
}