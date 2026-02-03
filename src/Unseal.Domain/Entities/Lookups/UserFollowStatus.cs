using System;
using System.Collections.Generic;
using Unseal.Entities.Users;

namespace Unseal.Entities.Lookups;

public class UserFollowStatus : LookupBaseEntity
{
    public UserFollowStatus(
        Guid id,
        string name,
        int code
    ) : base(id, name, code)
    {
    }
    public virtual ICollection<UserFollower>  UserFollowers { get; set; }
}