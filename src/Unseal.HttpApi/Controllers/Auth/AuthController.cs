using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unseal.Constants;
using Unseal.Dtos.Auth;
using Unseal.Permissions.Users;
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
    ) => await AuthAppService.RegisterAsync(registerDto, cancellationToken);

    /// <summary>
    /// Use to login
    /// </summary>
    /// <param name="loginDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<LoginResponseDto> LoginAsync(
        LoginDto loginDto,
        CancellationToken cancellationToken = default
    ) => await AuthAppService.LoginAsync(loginDto, cancellationToken);

    /// <summary>
    /// Use to logout
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("logout")]
    public async Task<bool> LoginAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    ) => await AuthAppService.LogoutAsync(refreshToken, cancellationToken);

    /// <summary>
    /// Used to resend the confirmation email if it could not be sent during registration. 
    /// </summary>
    /// <param name="mail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("send-confirmation-mail")]
    [Authorize(UserPermissions.Default)]
    public async Task<bool> SendConfirmationMailAsync(
        string mail,
        CancellationToken cancellationToken = default
    ) => await AuthAppService.SendConfirmationMailAsync(mail, cancellationToken);

    /// <summary>
    /// Use to change mail address.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="newMailAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    [Authorize]
    [HttpPost("change-mail")]
    [Authorize(UserPermissions.Update)]
    public async Task<bool> ChangeMailAsync(
        Guid userId,
        string newMailAddress,
        CancellationToken cancellationToken = default
    ) => await AuthAppService.ChangeMailAsync(userId, newMailAddress, cancellationToken);
    
    /// <summary>
    /// Used to confirm an mail address change.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="newMailAddress"></param>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(ApiConstants.Auth.ConfirmChangeMail)]
    [AllowAnonymous]
    public async Task<bool> ConfirmChangeMailAsync(
        Guid userId,
        string newMailAddress,
        string token,
        CancellationToken cancellationToken = default
    ) => await AuthAppService.ConfirmChangeMailAsync(
        userId,
        newMailAddress,
        token,
        cancellationToken
    );

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
    ) => await AuthAppService.ConfirmMailAsync(
        userId,
        token,
        cancellationToken
    );

    /// <summary>
    /// Used to confirm password reset.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="newPassword"></param>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(ApiConstants.Auth.ConfirmPasswordReset)]
    [AllowAnonymous]
    public async Task<bool> ConfirmPasswordResetAsync(
        Guid userId,
        string newPassword,
        string token,
        CancellationToken cancellationToken = default
    ) => await AuthAppService.ConfirmPasswordResetAsync(
        userId,
        newPassword,
        token,
        cancellationToken
    );
    /// <summary>
    /// Use to change password.
    /// </summary>
    /// <param name="changePasswordDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("change-password")]
    [Authorize(UserPermissions.Update)]
    public async Task<bool> ChangePasswordAsync(
        ChangePasswordDto changePasswordDto,
        CancellationToken cancellationToken=default
    ) => await AuthAppService.ChangePasswordAsync(
        changePasswordDto,
        cancellationToken
    );
    
    /// <summary>
    /// Use to delete user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpDelete("{userId}")]
    [Authorize(UserPermissions.Delete)]
    public async Task<bool> UserDeleteAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    ) => await AuthAppService.UserDeleteAsync(userId, cancellationToken);

    /// <summary>
    /// Use to deactivate account.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <summary>
    /// Use to deactivate account.
    /// </summary>
    [HttpPut("deactivate-account")]
    [Authorize(UserPermissions.Update)]
    public async Task<bool> DeactivateAccountAsync(
        [FromBody]string refreshToken,
        CancellationToken cancellationToken = default
    ) => await AuthAppService.DeactivateAccountAsync(refreshToken, cancellationToken);

    /// <summary>
    /// Use to send account activity mail.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("send-activation-mail")]
    [Authorize(UserPermissions.Default)]
    public async Task<bool> SendActivationMailAsync(
        string email,
        CancellationToken cancellationToken = default
    ) => await AuthAppService.SendActivityMailAsync(
        email,
        cancellationToken
    );
    
    /// <summary>
    /// Use to confirm activation mail.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(ApiConstants.Auth.ConfirmActivationMail)]
    [AllowAnonymous]
    public async Task<bool> ConfirmActivationMailAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    ) => await AuthAppService.ConfirmActivationMailAsync(
        userId,
        cancellationToken
    );
}