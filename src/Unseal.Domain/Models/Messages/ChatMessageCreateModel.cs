using System;
using System.Collections.Generic;

namespace Unseal.Models.Messages;

public class ChatMessageCreateModel
{
    public Guid SenderId { get; set; }
    public List<Guid> TargetIds { get; set; }
    public Guid ChatTypeId { get; set; }
    public string Content { get; set; }
}