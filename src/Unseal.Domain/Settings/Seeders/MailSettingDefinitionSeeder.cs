using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unseal.Constants;
using Unseal.Settings.Custom;
using Unseal.Settings.Models;

namespace Unseal.Settings.Seeders;

public class MailSettingDefinitionSeeder : SettingDefinitionSeeder<MailSettingModel>
{
    private readonly IConfiguration _configuration;
    
    public MailSettingDefinitionSeeder(
        ICustomSettingManager<MailSettingModel> customSettingManager,
        IConfiguration configuration) : 
        base(customSettingManager)
    {
        _configuration = configuration;
    }

    public async Task SeedDataAsync()
    {
        var mailSettings = _configuration
            .GetSection(SettingConstants.MailSettingModel.Gmail)
            .Get<MailSettings>();
        
        if (mailSettings is null) return;
        
        var model = new MailSettingModel
        {
            Server = new SettingItem { Name = SettingConstants.MailSettingModel.Server, Value = mailSettings.Server },
            Port = new SettingItem { Name = SettingConstants.MailSettingModel.Port, Value = mailSettings.Port },
            Login = new SettingItem { Name = SettingConstants.MailSettingModel.Login, Value = mailSettings.Login },
            Key = new SettingItem { Name = SettingConstants.MailSettingModel.Key, Value = mailSettings.Key }
        };
        await SeedAsync(model);
    }
}