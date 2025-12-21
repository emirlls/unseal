using Unseal.Localization;
using Volo.Abp.Application.Services;

namespace Unseal;

public abstract class UnsealAppService : ApplicationService
{
    protected UnsealAppService()
    {
        LocalizationResource = typeof(UnsealResource);
        ObjectMapperContext = typeof(UnsealApplicationModule);
    }
}
