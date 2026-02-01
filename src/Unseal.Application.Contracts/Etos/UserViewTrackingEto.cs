using System;
using System.Collections.Generic;

namespace Unseal.Etos;

public class UserViewTrackingEto
{
    public Guid UserId { get; set; }
    public List<Guid> CapsuleIds { get; set; }
}