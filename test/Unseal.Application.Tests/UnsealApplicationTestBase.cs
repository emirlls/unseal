using Volo.Abp.Modularity;

namespace Unseal;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class UnsealApplicationTestBase<TStartupModule> : UnsealTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
