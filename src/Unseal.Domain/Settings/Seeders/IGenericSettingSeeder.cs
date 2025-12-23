using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Unseal.Settings.Seeders;

public interface IGenericSettingSeeder : IDataSeedContributor,ITransientDependency
{
}