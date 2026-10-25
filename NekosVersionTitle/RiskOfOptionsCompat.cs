using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace NekosVersionTitle
{
    // Kept in its own class so RiskOfOptions types are only loaded when the mod is installed.
    internal static class RiskOfOptionsCompat
    {
        internal const string PluginGUID = "com.rune580.riskofoptions";

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static void Init(NekosVersionTitle plugin)
        {
            ModSettingsManager.SetModDescription("Neko's Version main menu logo and version text.");

            var icon = new Texture2D(2, 2);
            if (icon.LoadImage(NekosVersionTitle.ReadEmbeddedBytes(NekosVersionTitle.EmbeddedIconPath)))
            {
                ModSettingsManager.SetModIcon(Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), new Vector2(0.5f, 0.5f)));
            }

            // The default input field submits on every keystroke and trims trailing whitespace each time,
            // which swallows spaces while typing. Submit on Enter or when leaving the field instead.
            ModSettingsManager.AddOption(new CheckBoxOption(plugin.replaceLogo));
            ModSettingsManager.AddOption(new StringInputFieldOption(plugin.customLogoPath, new InputFieldConfig
            {
                submitOn = InputFieldConfig.SubmitEnum.OnExitOrSubmit,
                lineType = TMP_InputField.LineType.SingleLine,
                richText = false,
            }));
            ModSettingsManager.AddOption(new CheckBoxOption(plugin.showVersionText));
            ModSettingsManager.AddOption(new StringInputFieldOption(plugin.versionText, new InputFieldConfig
            {
                submitOn = InputFieldConfig.SubmitEnum.OnExitOrSubmit,
                lineType = TMP_InputField.LineType.SingleLine,
            }));
        }
    }
}
