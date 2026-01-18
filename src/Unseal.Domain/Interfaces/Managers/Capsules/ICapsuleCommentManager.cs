using System;
using Unseal.Entities.Capsules;

namespace Unseal.Interfaces.Managers.Capsules;

public interface ICapsuleCommentManager : IBaseDomainService<CapsuleComment>
{
    CapsuleComment Create(Guid capsuleId, Guid? currentUserId, string comment);
}