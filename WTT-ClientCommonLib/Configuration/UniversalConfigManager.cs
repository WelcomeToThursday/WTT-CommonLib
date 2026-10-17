using BepInEx.Configuration;
using WTTClientCommonLib.Attributes;

namespace WTTClientCommonLib.Configuration;

public static class UniversalConfigManager
{
    internal static ConfigEntry<bool> DeveloperMode { get; private set; }

    public static void Initialize(ConfigFile config)
    {
        DeveloperMode = config.Bind(
            "General",
            "Developer Mode",
#if DEBUG
            true,
#else
            false,
#endif
            new ConfigDescription(
                "Enable Developer Mode and RESTART YOUR GAME to access zone editor and static spawn system tools",
                null,
                new ConfigurationManagerAttributes { Order = 0 }
            )
        );
    }
}
