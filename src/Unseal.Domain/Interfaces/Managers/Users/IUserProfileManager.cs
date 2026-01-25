using Unseal.Entities.Users;
using Unseal.Models.Users;

namespace Unseal.Interfaces.Managers.Users;

public interface IUserProfileManager : IBaseDomainService<UserProfile>
{
    UserProfile CreateUserProfile(UserProfileCreateModel userProfileCreateModel);
    void UpdateUserProfile(UserProfile userProfile,
        UserProfileUpdateModel userProfileUpdateModel);
}