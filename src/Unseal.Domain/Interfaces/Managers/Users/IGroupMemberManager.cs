using System;
using System.Collections.Generic;
using Unseal.Entities.Users;
using Unseal.Models.Users;

namespace Unseal.Interfaces.Managers.Users;

public interface IGroupMemberManager : IBaseDomainService<GroupMember>
{
    List<GroupMember> Create(
        GroupCreateModel groupCreateModel,
        Guid groupId,
        Guid creatorId
    );
}