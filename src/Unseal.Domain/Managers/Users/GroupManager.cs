using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Users;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Models.Users;
using Unseal.Repositories.Base;

namespace Unseal.Managers.Users;

public class GroupManager : BaseDomainService<Group>, IGroupManager
{
    public GroupManager(
        IBaseRepository<Group> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer) : 
        base(baseRepository,
            stringLocalizer, 
            ExceptionCodes.Group.NotFound,
            ExceptionCodes.Group.AlreadyExists
        )
    {
    }

    public Group Create(GroupCreateModel groupCreateModel)
    {
        var group = new Group(
            GuidGenerator.Create(),
            groupCreateModel.Name,
            groupCreateModel.Description);
        return group;
    }

    public Group Update(Group group, GroupCreateModel groupUpdateModel)
    {
        group.Name = groupUpdateModel.Name;
        group.Description = groupUpdateModel.Description;
        return group;
    }
}