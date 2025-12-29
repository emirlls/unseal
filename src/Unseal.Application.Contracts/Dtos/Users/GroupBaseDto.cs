using System;
using System.Collections.Generic;

namespace Unseal.Dtos.Users;

public record GroupBaseDto(
    List<Guid> UserIds,
    string Name,
    string Description
);