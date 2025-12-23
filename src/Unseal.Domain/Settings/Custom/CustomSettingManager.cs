using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Constants;
using Volo.Abp.Json;
using Volo.Abp.SettingManagement;

namespace Unseal.Settings.Custom;

public class CustomSettingManager<TSetting> : ICustomSettingManager<TSetting>
where TSetting : class
{
    private readonly ISettingManager _settingManager;
    private readonly IJsonSerializer _jsonSerializer;

    public CustomSettingManager(
        IJsonSerializer jsonSerializer,
        ISettingManager settingManager
    )
    {
        _jsonSerializer = jsonSerializer;
        _settingManager = settingManager;
    }

    private string GetSettingName(string description) => $"{SettingConstants.Prefix}{description}";
    
    public async Task SetAsync(TSetting setting, CancellationToken cancellationToken = default)
    {
        var description = typeof(TSetting).GetCustomAttribute<DescriptionAttribute>()?.Description 
                          ?? typeof(TSetting).Name;
        var settingName = GetSettingName(description);
        var jsonValue = _jsonSerializer.Serialize(setting, camelCase:false);
        await _settingManager.SetGlobalAsync(settingName, jsonValue);
    }

    public async Task<TSetting> GetAsync(TSetting setting, CancellationToken cancellationToken = default)
    {
        var description = typeof(TSetting).GetCustomAttribute<DescriptionAttribute>()?.Description 
                          ?? typeof(TSetting).Name;
        var settingName = GetSettingName(description);
        var settings = await _settingManager.GetOrNullDefaultAsync(settingName);
        var model  = _jsonSerializer.Deserialize<TSetting>(settings, camelCase:false);
        return model;
    }

    public async Task<string> GetSerializedSettingAsync(
        TSetting settings,
        CancellationToken cancellationToken = default
    )
    {
        var jsonValue = _jsonSerializer.Serialize(settings);
        return jsonValue;
    }

    public async Task<bool> ExistsAsync(TSetting setting, CancellationToken cancellationToken = default)
    {
        var description = typeof(TSetting).GetCustomAttribute<DescriptionAttribute>()?.Description 
                          ?? typeof(TSetting).Name;
        var settingName = GetSettingName(description);
        var settings = await _settingManager.GetOrNullGlobalAsync(settingName);
        return !settings.IsNullOrWhiteSpace();
    }
    
}