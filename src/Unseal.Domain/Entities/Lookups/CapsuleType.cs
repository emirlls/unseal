using System;
using System.Collections.Generic;
using Unseal.Entities.Capsules;

namespace Unseal.Entities.Lookups;

public class CapsuleType : LookupBaseEntity
{
    public CapsuleType(Guid id, string name, int code) : base(id, name, code)
    {
    }
    public virtual ICollection<Capsule> Capsules { get; set; }
}