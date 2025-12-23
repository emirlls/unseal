using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Unseal.Settings.Seeders;

public interface ISettingDefinitionSeeder<TSetting>: ITransientDependency
where TSetting : class
{
    Task SeedAsync(TSetting setting, CancellationToken cancellationToken = default);
}