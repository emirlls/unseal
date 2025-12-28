using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Interfaces.Managers.Auth;
using Unseal.Localization;
using Unseal.Models.Auth;
using Unseal.Repositories.Base;
using Volo.Abp.Identity;

namespace Unseal.Managers.Auth;

public class CustomIdentityUserManager : BaseDomainService<IdentityUser>, ICustomIdentityUserManager
{
    
    public CustomIdentityUserManager(
        IBaseRepository<IdentityUser> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer) : 
        base(baseRepository,
            stringLocalizer,
            ExceptionCodes.IdentityUser.NotFound,
            ExceptionCodes.IdentityUser.AlreadyExists
        )
    {
    }

    public async Task<IdentityUser> Create(Guid id, Guid? tenantId, RegisterModel model)
    {
        var user = new IdentityUser(id, model.Email,model.Email,tenantId)
        {
            Name = model.FirstName,
            Surname = model.LastName
        };
        return user;
    }
    
}