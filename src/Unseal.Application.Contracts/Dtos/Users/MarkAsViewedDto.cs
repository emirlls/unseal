using System;
using System.Collections.Generic;

namespace Unseal.Dtos.Users;

public class MarkAsViewedDto
{
    public List<Guid> ExternalIds { get; set; }
    public Guid UserViewTrackingTypeId { get; set; }
}