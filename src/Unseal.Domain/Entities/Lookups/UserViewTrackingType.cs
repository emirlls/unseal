using System;
using System.Collections.Generic;
using Unseal.Entities.Users;

namespace Unseal.Entities.Lookups;

public class UserViewTrackingType : LookupBaseEntity
{
    public UserViewTrackingType(Guid id, string name, int code) : base(id, name, code)
    {
    }

    public virtual ICollection<UserViewTracking> UserViewTrackings { get; set; }
}