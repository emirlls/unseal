using System;
using System.Collections.Generic;

namespace Unseal.Dtos.Groups;

public record GroupDetailDto(
    Guid Id,
    string Name,
    string? Description,
    string? GroupImageUrl,
    List<GroupMemberDto> Members
    );