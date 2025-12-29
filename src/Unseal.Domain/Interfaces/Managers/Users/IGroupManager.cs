using Unseal.Entities.Users;
using Unseal.Models.Users;

namespace Unseal.Interfaces.Managers.Users;

public interface IGroupManager : IBaseDomainService<Group>
{
    Group Create(GroupCreateModel groupCreateModel);
    Group Update(Group group, GroupCreateModel groupUpdateModel);
}