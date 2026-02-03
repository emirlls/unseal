using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Groups;
using Unseal.Interfaces.Managers.Groups;
using Unseal.Localization;
using Unseal.Models.Groups;
using Unseal.Repositories.Groups;

namespace Unseal.Managers.Groups;

public class GroupMemberManager : BaseDomainService<GroupMember>, IGroupMemberManager
{
    public GroupMemberManager(
        IGroupMemberRepository baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer) : 
        base(baseRepository,
            stringLocalizer,
            ExceptionCodes.GroupMember.NotFound,
            ExceptionCodes.GroupMember.AlreadyExists
        )
    {
    }

    public List<GroupMember> Create(
        GroupCreateModel groupCreateModel, 
        Guid groupId,
        Guid creatorId
    )
    {
        groupCreateModel.UserIds.Add(creatorId);
        var groupMembers = new List<GroupMember>();
        foreach (var userId in groupCreateModel.UserIds)
        {
            var groupMember = new GroupMember(
                GuidGenerator.Create(),
                groupId,
                userId
            )
            {
                JoinDate = DateTime.Now
            };
            groupMembers.Add(groupMember);
        }

        return groupMembers;
    }
}