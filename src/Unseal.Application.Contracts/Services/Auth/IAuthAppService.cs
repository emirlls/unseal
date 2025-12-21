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
}