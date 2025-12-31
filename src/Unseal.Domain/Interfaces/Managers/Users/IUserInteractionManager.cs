using System;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Entities.Users;

namespace Unseal.Interfaces.Managers.Users;

public interface IUserInteractionManager : IBaseDomainService<UserInteraction>
{
    Task<bool> CheckUserBlockedAsync(
        Guid? sourceUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default
    );

    UserInteraction Create(Guid sourceUserId, Guid targetUserId);
}