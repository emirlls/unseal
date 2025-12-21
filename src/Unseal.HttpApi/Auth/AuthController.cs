using Volo.Abp.DependencyInjection;

namespace Unseal.Auth;

public class AuthController : UnsealController
{
    private IAbpLazyServiceProvider _lazyServiceProvider;

    public AuthController(IAbpLazyServiceProvider lazyServiceProvider)
    {
        _lazyServiceProvider = lazyServiceProvider;
    }
}