using System.ComponentModel;
using Unseal.Constants;

namespace Unseal.Settings.Models.ElasticSearch;

[Description(SettingConstants.ElasticSearchSettingModel.Name)]
public class ElasticSearchSettingModel
{
    public SettingItem Url { get; set; } = new();
    public SettingItem Username { get; set; } = new();
    public SettingItem Password { get; set; } = new();
}