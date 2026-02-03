namespace Unseal.Models.Users;

public record UserProfileUpdateModel(
    bool IsLocked,
    bool AllowJoinGroup,
    string? Content,
    string? ProfilePictureUrl
) : UserProfileBaseModel(
    Content,
    ProfilePictureUrl
);