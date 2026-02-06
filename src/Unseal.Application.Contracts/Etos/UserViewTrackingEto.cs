using System;
using System.Collections.Generic;

namespace Unseal.Etos;

public class UserViewTrackingEto
{
    public Guid UserId { get; set; }
    public Guid? UserViewTrackingTypeId { get; set; } 
    public List<Guid> ExternalIds { get; set; }
}