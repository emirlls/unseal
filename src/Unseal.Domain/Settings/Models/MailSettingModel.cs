using System.ComponentModel;
using Unseal.Constants;

namespace Unseal.Settings.Models;

[Description(SettingConstants.MailSettingModel.Name)]
public class MailSettingModel
{
    public SettingItem Server { get; set; } = new();
    public SettingItem Port { get; set; } = new();
    public SettingItem Login { get; set; } = new();
    public SettingItem Key { get; set; } = new();
}
