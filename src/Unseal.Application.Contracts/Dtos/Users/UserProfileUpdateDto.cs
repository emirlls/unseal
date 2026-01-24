using Volo.Abp.Content;

namespace Unseal.Dtos.Users;

public record UserProfileUpdateDto(
    bool IsLocked,
    bool AllowJoinGroup,
    string? Content,
    IRemoteStreamContent? StreamContent
) : UserProfileBaseDto(
    Content
);