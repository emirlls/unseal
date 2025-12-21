using Volo.Abp.Modularity;

namespace Unseal;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class UnsealDomainTestBase<TStartupModule> : UnsealTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
