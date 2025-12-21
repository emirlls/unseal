using Unseal.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Unseal;

public abstract class UnsealController : AbpControllerBase
{
    protected UnsealController()
    {
        LocalizationResource = typeof(UnsealResource);
    }
}
