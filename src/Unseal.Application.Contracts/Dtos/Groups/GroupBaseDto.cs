using System;
using System.Collections.Generic;

namespace Unseal.Dtos.Groups;

public record GroupBaseDto(
    List<Guid> UserIds,
    string Name,
    string? Description
);