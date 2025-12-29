using System;
using Unseal.Entities.Capsules;
using Unseal.Models.Capsules;

namespace Unseal.Interfaces.Managers.Capsules;

public interface ICapsuleManager : IBaseDomainService<Capsule>
{
    Capsule Create(CapsuleCreateModel capsuleCreateModel, Guid? creatorId);
}