using System;
using System.Collections.Generic;
using Unseal.Entities.Groups;
using Unseal.Models.Groups;

namespace Unseal.Interfaces.Managers.Groups;

public interface IGroupMemberManager : IBaseDomainService<GroupMember>
{
    List<GroupMember> Create(
        GroupCreateModel groupCreateModel,
        Guid groupId,
        Guid creatorId
    );
}