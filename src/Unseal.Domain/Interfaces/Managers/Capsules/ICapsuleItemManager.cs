using System;
using Unseal.Entities.Capsules;
using Unseal.Models.Capsules;

namespace Unseal.Interfaces.Managers.Capsules;

public interface ICapsuleItemManager : IBaseDomainService<CapsuleItem>
{
    CapsuleItem Create(CapsuleCreateModel capsuleCreateModel,Guid capsuleId);
}