using Unseal.Repositories.Base;
using Volo.Abp.Identity;

namespace Unseal.Repositories.Auth;

public interface ICustomRoleRepository : IBaseRepository<IdentityRole>
{
}