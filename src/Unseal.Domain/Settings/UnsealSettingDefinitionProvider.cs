using Unseal.Constants;
using Volo.Abp.Settings;

namespace Unseal.Settings;

public class UnsealSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        /* Define module settings here.
         * Use names from UnsealSettings class.
         */
        var mailSettingsName = $"{SettingConstants.Prefix}{SettingConstants.MailSettingModel.Name}";
        context.Add(
            new SettingDefinition(mailSettingsName)
        );
    }
}