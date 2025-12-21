using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Unseal.Samples;

public interface ISampleAppService : IApplicationService
{
    Task<SampleDto> GetAsync();

    Task<SampleDto> GetAuthorizedAsync();
}
