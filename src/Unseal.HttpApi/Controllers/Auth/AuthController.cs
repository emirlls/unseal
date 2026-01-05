using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unseal.Constants;
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
    /// Used to resend the confirmation email if it could not be sent during registration. 
    /// </summary>
    /// <param name="mail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("send-confirmation-mail")]
    public async Task<bool> SendConfirmationMailAsync(
        string mail,
        CancellationToken cancellationToken = default
    )
    {
        return await AuthAppService.SendConfirmationMailAsync(mail, cancellationToken);
    }

    /// <summary>
    /// Use to change mail address.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="newMailAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("change-mail")]
    public async Task<bool> ChangeMailAsync(
        Guid userId,
        string newMailAddress,
        CancellationToken cancellationToken = default
    )
    {
        return await AuthAppService.ChangeMailAsync(userId, newMailAddress, cancellationToken);
    }

    /// <summary>
    /// Used to confirm an mail address change.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="newMailAddress"></param>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(ApiConstants.Auth.ConfirmChangeMail)]
    public async Task<bool> ConfirmChangeMailAsync(
        Guid userId,
        string newMailAddress,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        return await AuthAppService.ConfirmChangeMailAsync(
            userId,
            newMailAddress,
            token,
            cancellationToken
        );
    }

    /// <summary>
    /// Use to confirm mail.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(ApiConstants.Auth.ConfirmMail)]
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
    [HttpDelete("{userId}")]
    public async Task<bool> UserDeleteAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await AuthAppService.UserDeleteAsync(userId, cancellationToken);
    }
}