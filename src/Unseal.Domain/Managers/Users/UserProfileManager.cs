using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Users;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Repositories.Base;

namespace Unseal.Managers.Users;

public class UserProfileManager : BaseDomainService<UserProfile>, IUserProfileManager
{
    public UserProfileManager(IBaseRepository<UserProfile> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer) : 
        base(baseRepository,
            stringLocalizer,
            ExceptionCodes.UserProfile.NotFound, 
            ExceptionCodes.UserProfile.AlreadyExists
        )
    {
    }
}