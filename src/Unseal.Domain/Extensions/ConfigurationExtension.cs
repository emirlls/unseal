using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Unseal.Extensions;

public static class ConfigurationExtension
{
    extension(IServiceProvider serviceProvider)
    {
        public async Task<IConfiguration> GetConfigurationAsync()
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            return configuration;
        }
        public async Task<string?> GetSelfUrlAsync()
        {
            var configuration = await serviceProvider.GetConfigurationAsync();
            var selfUrl = configuration["App:SelfUrl"];
            return selfUrl;
        }
    }
}