using System;
using System.Collections.Generic;

namespace Unseal.Dtos.Groups;

public record GroupUpdateDto(
    List<Guid> UserIds,
    string Name,
    string Description
    ) : 
    GroupBaseDto(UserIds, Name, Description);