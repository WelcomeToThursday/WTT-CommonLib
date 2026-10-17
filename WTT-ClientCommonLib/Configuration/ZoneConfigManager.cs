using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;
using WTTClientCommonLib.Attributes;
using WTTClientCommonLib.Helpers;
using WTTClientCommonLib.Models;

namespace WTTClientCommonLib.Configuration;

public static class ZoneConfigManager
{
    internal static Color ColorZoneRed = new(1f, 0f, 0f, 0.7f);
    internal static Color ColorZoneGreen = new(0f, 1f, 0f, 0.7f);
    public static List<CustomQuestZone> ExistingQuestZones { get; set; } = [];
    public static ConfigEntry<string> NewZoneName { get; private set; }
    public static ConfigEntry<string> NewZoneType { get; private set; }
    public static ConfigEntry<string> FlareZoneType { get; private set; }

    public static ConfigEntry<float> ZoneAdjustmentValue { get; private set; }

    public static ConfigEntry<float> PositionConfigX { get; private set; }
    public static ConfigEntry<float> PositionConfigY { get; private set; }
    public static ConfigEntry<float> PositionConfigZ { get; private set; }

    public static ConfigEntry<float> ScaleConfigX { get; private set; }
    public static ConfigEntry<float> ScaleConfigY { get; private set; }
    public static ConfigEntry<float> ScaleConfigZ { get; private set; }

    public static ConfigEntry<float> RotationConfigX { get; private set; }
    public static ConfigEntry<float> RotationConfigY { get; private set; }
    public static ConfigEntry<float> RotationConfigZ { get; private set; }

    public static void Initialize(ConfigFile config)
    {
        // Only create zone configs if Developer Mode is enabled
        if (!UniversalConfigManager.DeveloperMode.Value)
        {
            return;
        }

        // Section 1: Create CustomQuestZone
        NewZoneName = config.Bind(
            "1. Create CustomQuestZone",
            "CustomQuestZone ID",
            "",
            new ConfigDescription("The name for the new zone", null, new ConfigurationManagerAttributes { Order = 4 })
        );
        NewZoneType = config.Bind(
            "1. Create CustomQuestZone",
            "CustomQuestZone Type",
            "",
            new ConfigDescription(
                "Select the type of zone",
                ZoneUiHelpers.AcceptableTypes,
                new ConfigurationManagerAttributes { Order = 3 }
            )
        );
        FlareZoneType = config.Bind(
            "1. Create CustomQuestZone",
            "(Optional) Flare Type",
            "",
            new ConfigDescription(
                "Select the flare zone type",
                ZoneUiHelpers.AcceptableFlareTypes,
                new ConfigurationManagerAttributes { Order = 2 }
            )
        );
        config.Bind(
            "1. Create CustomQuestZone",
            "Add CustomQuestZone",
            "New CustomQuestZone",
            new ConfigDescription(
                "Adds a new zone with the zone ID",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.NewZoneDrawer, Order = 1 }
            )
        );

        // Section 2: Select CustomQuestZone
        config.Bind(
            "2. Select CustomQuestZone",
            "Navigate Zones",
            "",
            new ConfigDescription(
                "The ID for the currently selected CustomQuestZone.",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.SwitchZoneDrawer, ReadOnly = true }
            )
        );

        // Section 3: Adjustment Settings
        ZoneAdjustmentValue = config.Bind(
            "3.1. Adjustment Settings",
            "CustomQuestZone Adjustment Value",
            0.25f,
            new ConfigDescription("Sets the value used to adjust the position, scale and rotation of zones.")
        );

        PositionConfigX = config.Bind(
            "3.2. Change Position",
            "Change Position X",
            0f,
            new ConfigDescription(
                "Change the position of the current zone",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.PositionXDrawer, ReadOnly = true }
            )
        );
        PositionConfigY = config.Bind(
            "3.2. Change Position",
            "Change Position Y",
            0f,
            new ConfigDescription(
                "Change the position of the current zone",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.PositionYDrawer, ReadOnly = true }
            )
        );
        PositionConfigZ = config.Bind(
            "3.2. Change Position",
            "Change Position Z",
            0f,
            new ConfigDescription(
                "Change the position of the current zone",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.PositionZDrawer, ReadOnly = true }
            )
        );

        ScaleConfigX = config.Bind(
            "3.3. Change Scale",
            "Change Scale X",
            0f,
            new ConfigDescription(
                "Change the scale of the current zone",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.ScaleXDrawer, ReadOnly = true }
            )
        );
        ScaleConfigY = config.Bind(
            "3.3. Change Scale",
            "Change Scale Y",
            0f,
            new ConfigDescription(
                "Change the scale of the current zone",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.ScaleYDrawer, ReadOnly = true }
            )
        );
        ScaleConfigZ = config.Bind(
            "3.3. Change Scale",
            "Change Scale Z",
            0f,
            new ConfigDescription(
                "Change the scale of the current zone",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.ScaleZDrawer, ReadOnly = true }
            )
        );

        RotationConfigX = config.Bind(
            "3.4. Change Rotation",
            "Change Rotation X",
            0f,
            new ConfigDescription(
                "Change the rotation of the current zone",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.RotationXDrawer, ReadOnly = true }
            )
        );
        RotationConfigY = config.Bind(
            "3.4. Change Rotation",
            "Change Rotation Y",
            0f,
            new ConfigDescription(
                "Change the rotation of the current zone",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.RotationYDrawer, ReadOnly = true }
            )
        );
        RotationConfigZ = config.Bind(
            "3.4. Change Rotation",
            "Change Rotation Z",
            0f,
            new ConfigDescription(
                "Change the rotation of the current zone",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.RotationZDrawer, ReadOnly = true }
            )
        );

        // Section 4: Output
        config.Bind(
            "4. Output",
            "Output Zones",
            "",
            new ConfigDescription(
                "Outputs all zones to the root SPT directory.",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.OutputDrawer }
            )
        );

        // Section 5: View
        config.Bind(
            "5. View Zones",
            "Add Existing Zones",
            false,
            new ConfigDescription(
                "Adds any currently loaded custom zones to the editor.",
                null,
                new ConfigurationManagerAttributes { CustomDrawer = ZoneUiHelpers.ViewZonesDrawer }
            )
        );

        // Reset values to defaults
        NewZoneName.Value = (string)NewZoneName.DefaultValue;
        NewZoneType.Value = (string)NewZoneType.DefaultValue;
        FlareZoneType.Value = (string)FlareZoneType.DefaultValue;

        PositionConfigX.Value = (float)PositionConfigX.DefaultValue;
        PositionConfigY.Value = (float)PositionConfigY.DefaultValue;
        PositionConfigZ.Value = (float)PositionConfigZ.DefaultValue;

        ScaleConfigX.Value = (float)ScaleConfigX.DefaultValue;
        ScaleConfigY.Value = (float)ScaleConfigY.DefaultValue;
        ScaleConfigZ.Value = (float)ScaleConfigZ.DefaultValue;

        RotationConfigX.Value = (float)RotationConfigX.DefaultValue;
        RotationConfigY.Value = (float)RotationConfigY.DefaultValue;
        RotationConfigZ.Value = (float)RotationConfigZ.DefaultValue;
    }
}
