using System;
using System.Collections.Generic;
using Volo.Abp.Content;

namespace Unseal.Dtos.Groups;

public record GroupCreateDto(
    List<Guid> UserIds,
    string Name,
    string Description,
    IRemoteStreamContent? StreamContent
    ) 
    : GroupBaseDto(UserIds, Name, Description);