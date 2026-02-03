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
    private readonly IdentityUserManager _identityUserManager;
    public CustomIdentityUserManager(
        IBaseRepository<IdentityUser> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer, 
        IdentityUserManager identityUserManager
    ) : 
        base(baseRepository,
            stringLocalizer,
            ExceptionCodes.IdentityUser.NotFound,
            ExceptionCodes.IdentityUser.AlreadyExists
        )
    {
        _identityUserManager = identityUserManager;
    }

    public async Task<IdentityUser> Create(Guid id, Guid? tenantId, RegisterModel model)
    {
        var userName = await _identityUserManager.GetUserNameFromEmailAsync(model.Email);
        var user = new IdentityUser(id, userName ,model.Email,tenantId)
        {
            Name = model.FirstName,
            Surname = model.LastName
        };
        return user;
    }
    
}