using System;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Users;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Models.Users;
using Unseal.Repositories.Users;

namespace Unseal.Managers.Users;

public class UserProfileManager : BaseDomainService<UserProfile>, IUserProfileManager
{
    public UserProfileManager(
        IUserProfileRepository baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer) : 
        base(baseRepository,
            stringLocalizer,
            ExceptionCodes.UserProfile.NotFound, 
            ExceptionCodes.UserProfile.AlreadyExists
        )
    {
    }

    public UserProfile CreateUserProfile(UserProfileCreateModel userProfileCreateModel)
    {
        var userProfile = new UserProfile(
            GuidGenerator.Create(),
            userProfileCreateModel.UserId,
            userProfileCreateModel.Content,
            userProfileCreateModel.ProfilePictureUrl,
            DateTime.Now
        );
        return userProfile;
    }

    public void UpdateUserProfile(UserProfile userProfile,
        UserProfileUpdateModel userProfileUpdateModel)
    {
        userProfile.IsLocked = userProfileUpdateModel.IsLocked;
        userProfile.AllowJoinGroup = userProfileUpdateModel.AllowJoinGroup;
        userProfile.Content = userProfileUpdateModel.Content;
        userProfile.ProfilePictureUrl = userProfileUpdateModel.ProfilePictureUrl;
    }
}