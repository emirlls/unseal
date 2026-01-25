using System;

namespace Unseal.Models.Users;

public record UserProfileCreateModel(
    Guid UserId,
    string? Content,
    string? ProfilePictureUrl,
    DateTime LastActivityTime
) : UserProfileBaseModel(
    Content,
    ProfilePictureUrl
);