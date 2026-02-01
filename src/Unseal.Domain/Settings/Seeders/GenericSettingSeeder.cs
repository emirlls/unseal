using System.Threading.Tasks;
using Volo.Abp.Data;

namespace Unseal.Settings.Seeders;

public class GenericSettingSeeder : IGenericSettingSeeder
{
    //Add seeder services here.
    private readonly MailSettingDefinitionSeeder _mailSettingDefinitionSeeder;
    private readonly ElasticSearchSettingDefinitionSeeder _elasticSearchSettingDefinitionSeeder;

    public GenericSettingSeeder(
        MailSettingDefinitionSeeder mailSettingDefinitionSeeder, 
        ElasticSearchSettingDefinitionSeeder elasticSearchSettingDefinitionSeeder
    )
    {
        _mailSettingDefinitionSeeder = mailSettingDefinitionSeeder;
        _elasticSearchSettingDefinitionSeeder = elasticSearchSettingDefinitionSeeder;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await _mailSettingDefinitionSeeder.SeedDataAsync();
        await _elasticSearchSettingDefinitionSeeder.SeedDataAsync();
    }
}