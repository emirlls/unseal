using Unseal.Entities.Groups;
using Unseal.Models.Users;

namespace Unseal.Interfaces.Managers.Groups;

public interface IGroupManager : IBaseDomainService<Group>
{
    Group Create(GroupCreateModel groupCreateModel);
    Group Update(Group group, GroupCreateModel groupUpdateModel);
}