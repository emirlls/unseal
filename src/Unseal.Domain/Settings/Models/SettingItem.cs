using System.ComponentModel;

namespace Unseal.Settings.Models;

[Description()]
public class SettingItem
{
    public string? Name { get; set; }
    public string? LocalizedName { get; set; }
    public object? Value { get; set; }
}