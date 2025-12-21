using System;
using System.Threading.Tasks;
using Unseal.Models.Auth;
using Volo.Abp.Identity;

namespace Unseal.Interfaces.Managers.Auth;

public interface ICustomIdentityUserManager : IBaseDomainService<IdentityUser>
{
    Task<IdentityUser> Create(Guid id, Guid? tenantId, RegisterModel model);
}