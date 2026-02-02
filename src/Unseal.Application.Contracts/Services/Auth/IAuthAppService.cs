using System;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Dtos.Auth;
using Volo.Abp.Application.Services;

namespace Unseal.Services.Auth;

public interface IAuthAppService : IApplicationService
{
    Task<bool> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default
    );

    Task<LoginResponseDto> LoginAsync(
        LoginDto loginDto,
        CancellationToken cancellationToken = default);

    Task<bool> ConfirmMailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default
    );

    Task<bool> UserDeleteAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    Task<bool> SendConfirmationMailAsync(
        string mail,
        CancellationToken cancellationToken
    );

    Task<bool> ChangeMailAsync(
        Guid userId,
        string newMailAddress,
        CancellationToken cancellationToken = default
    );

    Task<bool> ConfirmChangeMailAsync(
        Guid userId,
        string newMailAddress,
        string token,
        CancellationToken cancellationToken = default
    );
    
    Task<bool> ChangePasswordAsync(
        ChangePasswordDto changePasswordDto,
        CancellationToken cancellationToken = default
    );

    Task<bool> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    );
}