using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unseal.Constants;
using Unseal.Settings.Custom;
using Unseal.Settings.Models;
using Unseal.Settings.Models.ElasticSearch;

namespace Unseal.Settings.Seeders;

public class ElasticSearchSettingDefinitionSeeder : SettingDefinitionSeeder<ElasticSearchSettingModel>
{
    private readonly IConfiguration _configuration;
    
    public ElasticSearchSettingDefinitionSeeder(
        ICustomSettingManager<ElasticSearchSettingModel> customSettingManager,
        IConfiguration configuration
    ) : base(customSettingManager)
    {
        _configuration = configuration;
    }
    
    public async Task SeedDataAsync()
    {
        var elasticSearchSettings = _configuration
            .GetSection(SettingConstants.ElasticSearchSettingModel.Name)
            .Get<ElasticSearchSettings>();
        
        if (elasticSearchSettings is null) return;
        
        var model = new ElasticSearchSettingModel
        {
            Url = new SettingItem { Name = SettingConstants.ElasticSearchSettingModel.Url, Value = elasticSearchSettings.Url },
            Username = new SettingItem { Name = SettingConstants.ElasticSearchSettingModel.Username, Value = elasticSearchSettings.Username },
            Password = new SettingItem { Name = SettingConstants.ElasticSearchSettingModel.Password, Value = elasticSearchSettings.Password }
        };
        await SeedAsync(model);
    }
}