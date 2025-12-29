using System;
using System.Collections.Generic;

namespace Unseal.Dtos.Users;

public record GroupUpdateDto(
    List<Guid> UserIds,
    string Name,
    string Description
    ) : 
    GroupBaseDto(UserIds, Name, Description);