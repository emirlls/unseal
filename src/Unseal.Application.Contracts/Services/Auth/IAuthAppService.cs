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
        CancellationToken cancellationToken =  default
    );

    Task<bool> UserDeleteAsync(
        Guid userId,
        CancellationToken cancellationToken=default
    );
}