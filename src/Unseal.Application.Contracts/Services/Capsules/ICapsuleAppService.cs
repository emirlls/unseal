using System.Threading;
using System.Threading.Tasks;
using Unseal.Dtos.Capsules;
using Volo.Abp.Application.Services;

namespace Unseal.Services.Capsules;

public interface ICapsuleAppService : IApplicationService
{
    Task<bool> CreateAsync(
        CapsuleCreateDto capsuleCreateDto,
        CancellationToken cancellationToken = default
    );
}