using System;
using System.Collections.Generic;
using Unseal.Entities.Messages;

namespace Unseal.Entities.Lookups;

public class ChatType : LookupBaseEntity
{
    public ChatType(
        Guid id,
        string name,
        int code
        ) : base(
        id,
        name,
        code
    )
    {
    }
    public virtual ICollection<ChatMessage> ChatMessages { get; set; }
}