using System;
using Unseal.Entities.Capsules;

namespace Unseal.Interfaces.Managers.Capsules;

public interface ICapsuleLikeManager : IBaseDomainService<CapsuleLike>
{
    CapsuleLike Create(Guid capsuleId, Guid? currentUserId);
}