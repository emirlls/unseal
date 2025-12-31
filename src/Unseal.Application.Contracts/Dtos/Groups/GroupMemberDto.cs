using System;

namespace Unseal.Dtos.Groups;

public record GroupMemberDto(
    Guid UserId,
    string Name,
    string Surname
    );