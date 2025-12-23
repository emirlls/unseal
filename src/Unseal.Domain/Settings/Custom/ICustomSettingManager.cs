using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Unseal.Settings.Custom;

public interface ICustomSettingManager<TSetting> : ITransientDependency
where TSetting: class
{
    Task SetAsync(
        TSetting settings,
        CancellationToken cancellationToken = default
    );
    Task<TSetting> GetAsync(
        TSetting settings,
        CancellationToken cancellationToken = default
    );
    
    Task<string> GetSerializedSettingAsync(
        TSetting settings,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsAsync(
        TSetting setting,
        CancellationToken cancellationToken = default
    );
}