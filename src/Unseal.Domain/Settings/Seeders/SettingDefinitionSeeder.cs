using System.Threading;
using System.Threading.Tasks;
using Unseal.Settings.Custom;

namespace Unseal.Settings.Seeders;

public abstract class SettingDefinitionSeeder<TSetting> : ISettingDefinitionSeeder<TSetting>
where TSetting : class
{

    private readonly ICustomSettingManager<TSetting> _customSettingManager;

    public SettingDefinitionSeeder(
        ICustomSettingManager<TSetting> customSettingManager
    )
    {
        _customSettingManager = customSettingManager;
    }

    public async Task SeedAsync(TSetting setting, CancellationToken cancellationToken = default)
    {
        var settingExists = await _customSettingManager.ExistsAsync(setting, cancellationToken);
        if(settingExists) return;
        await _customSettingManager.SetAsync(setting, cancellationToken);
    }
    
}