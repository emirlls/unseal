using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Lookups;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Repositories.Lookups;

namespace Unseal.Managers.Users;

public class UserFollowStatusManager : BaseDomainService<UserFollowStatus>, IUserFollowStatusManager
{
    public UserFollowStatusManager(
        IUserFollowStatusRepository baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
    ) : base(
        baseRepository,
        stringLocalizer,
        ExceptionCodes.UserFollowStatus.NotFound,
        ExceptionCodes.UserFollowStatus.AlreadyExists
    )
    {
    }
}