using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unseal.Dtos.Auth;
using Unseal.Services.Auth;
using Volo.Abp.DependencyInjection;

namespace Unseal.Auth;

[Route("api/auth")]
[ApiController]
public class AuthController : UnsealController
{
    private readonly IAbpLazyServiceProvider _lazyServiceProvider;

    public AuthController(IAbpLazyServiceProvider lazyServiceProvider)
    {
        _lazyServiceProvider = lazyServiceProvider;
    }

    private IAuthAppService AuthAppService =>
        _lazyServiceProvider.LazyGetRequiredService<IAuthAppService>();

    /// <summary>
    /// Use to register.
    /// </summary>
    /// <param name="registerDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<bool> RegisterAsync(
        RegisterDto registerDto,
        CancellationToken cancellationToken = default
    )
    {
        return await AuthAppService.RegisterAsync(registerDto, cancellationToken);
    }

    /// <summary>
    /// Use to login
    /// </summary>
    /// <param name="loginDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        return await AuthAppService.LoginAsync(loginDto, cancellationToken);
    }
}