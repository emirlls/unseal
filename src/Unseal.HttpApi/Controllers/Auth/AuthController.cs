using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unseal.Dtos.Auth;
using Unseal.Services.Auth;
using Volo.Abp.DependencyInjection;

namespace Unseal.Controllers.Auth;

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

    /// <summary>
    /// Use to confirm mail
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<bool> ConfirmMailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        return await AuthAppService.ConfirmMailAsync(
            userId,
            token,
            cancellationToken
        );
    }

    /// <summary>
    /// Use to delete user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("user-delete")]
    public async Task<bool> UserDeleteAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await AuthAppService.UserDeleteAsync(userId, cancellationToken);
    }
}