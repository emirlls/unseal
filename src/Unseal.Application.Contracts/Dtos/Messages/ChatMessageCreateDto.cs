using System;
using System.Collections.Generic;

namespace Unseal.Dtos.Messages;

public class ChatMessageCreateDto
{
    public List<Guid> TargetIds { get; set; } // user or group id
    public Guid ChatTypeId { get; set; } // directly or group
    public string Content { get; set; }
}