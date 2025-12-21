using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Unseal.Dtos.Auth;
using Unseal.Interfaces.Managers.Auth;
using Unseal.Profiles.Auth;
using Volo.Abp.Identity;

namespace Unseal.Services.Auth;

public class AuthAppService : UnsealAppService, IAuthAppService
{
    private IdentityUserManager IdentityUserManager =>
        LazyServiceProvider.LazyGetRequiredService<IdentityUserManager>();
    private ICustomIdentityUserManager CustomIdentityUserManager =>
        LazyServiceProvider.LazyGetRequiredService<ICustomIdentityUserManager>();

    private AuthMapper AuthMapper =>
        LazyServiceProvider.LazyGetRequiredService<AuthMapper>();
    public async Task<bool> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var model = AuthMapper.MapToModel(dto);
        
        var user =await CustomIdentityUserManager.Create(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            model
        );
        var result = await IdentityUserManager.CreateAsync(user, model.Password);
        result.CheckErrors();
        await IdentityUserManager.AddDefaultRolesAsync(user);
        return result.Succeeded;
    }
}