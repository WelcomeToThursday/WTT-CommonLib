using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;
using WTTClientCommonLib.Configuration;
using WTTClientCommonLib.Models;
using WTTClientCommonLib.Services;

namespace WTTClientCommonLib.Helpers;

public static class ZoneUiHelpers
{
    public static readonly AcceptableValueList<string> AcceptableTypes = new("placeitem", "visit", "flarezone", "botkillzone", "salvage");

    public static readonly AcceptableValueList<string> AcceptableFlareTypes = new(
        "",
        "Light",
        "Airdrop",
        "ExitActivate",
        "Quest",
        "AIFollowEvent"
    );

    public static List<CustomQuestZone> ExistingQuestZones = new();

    public static void ViewZonesDrawer(ConfigEntryBase entry)
    {
        if (GUILayout.Button("Add Existing Zones"))
            ZoneService.AddExistingZones();
    }

    public static void NewZoneDrawer(ConfigEntryBase entry)
    {
        if (GUILayout.Button("New CustomQuestZone", GUILayout.Width(100)))
            ZoneService.CreateNewZone();
    }

    public static void SwitchZoneDrawer(ConfigEntryBase entry)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Prev", GUILayout.Width(45)))
            ZoneService.PrevZone();
        GUILayout.TextField(ZoneService.GetCurrentZoneName(), GUILayout.ExpandWidth(true));
        if (GUILayout.Button("Next", GUILayout.Width(45)))
            ZoneService.NextZone();
        GUILayout.EndHorizontal();
    }

    public static void PositionXDrawer(ConfigEntryBase entry)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<--", GUILayout.Width(45)))
        {
            ZoneConfigManager.PositionConfigX.Value -= ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustPosition();
        }

        GUILayout.TextField(ZoneConfigManager.PositionConfigX.Value.ToString(), GUILayout.ExpandWidth(true));
        if (GUILayout.Button("-->", GUILayout.Width(45)))
        {
            ZoneConfigManager.PositionConfigX.Value += ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustPosition();
        }

        GUILayout.EndHorizontal();
    }

    public static void PositionYDrawer(ConfigEntryBase entry)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<--", GUILayout.Width(45)))
        {
            ZoneConfigManager.PositionConfigY.Value -= ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustPosition();
        }

        GUILayout.TextField(ZoneConfigManager.PositionConfigY.Value.ToString(), GUILayout.ExpandWidth(true));
        if (GUILayout.Button("-->", GUILayout.Width(45)))
        {
            ZoneConfigManager.PositionConfigY.Value += ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustPosition();
        }

        GUILayout.EndHorizontal();
    }

    public static void PositionZDrawer(ConfigEntryBase entry)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<--", GUILayout.Width(45)))
        {
            ZoneConfigManager.PositionConfigZ.Value -= ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustPosition();
        }

        GUILayout.TextField(ZoneConfigManager.PositionConfigZ.Value.ToString(), GUILayout.ExpandWidth(true));
        if (GUILayout.Button("-->", GUILayout.Width(45)))
        {
            ZoneConfigManager.PositionConfigZ.Value += ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustPosition();
        }

        GUILayout.EndHorizontal();
    }

    public static void ScaleXDrawer(ConfigEntryBase entry)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<--", GUILayout.Width(45)))
        {
            ZoneConfigManager.ScaleConfigX.Value -= ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustScale();
        }

        GUILayout.TextField(ZoneConfigManager.ScaleConfigX.Value.ToString(), GUILayout.ExpandWidth(true));
        if (GUILayout.Button("-->", GUILayout.Width(45)))
        {
            ZoneConfigManager.ScaleConfigX.Value += ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustScale();
        }

        GUILayout.EndHorizontal();
    }

    public static void ScaleYDrawer(ConfigEntryBase entry)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<--", GUILayout.Width(45)))
        {
            ZoneConfigManager.ScaleConfigY.Value -= ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustScale();
        }

        GUILayout.TextField(ZoneConfigManager.ScaleConfigY.Value.ToString(), GUILayout.ExpandWidth(true));
        if (GUILayout.Button("-->", GUILayout.Width(45)))
        {
            ZoneConfigManager.ScaleConfigY.Value += ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustScale();
        }

        GUILayout.EndHorizontal();
    }

    public static void ScaleZDrawer(ConfigEntryBase entry)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<--", GUILayout.Width(45)))
        {
            ZoneConfigManager.ScaleConfigZ.Value -= ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustScale();
        }

        GUILayout.TextField(ZoneConfigManager.ScaleConfigZ.Value.ToString(), GUILayout.ExpandWidth(true));
        if (GUILayout.Button("-->", GUILayout.Width(45)))
        {
            ZoneConfigManager.ScaleConfigZ.Value += ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustScale();
        }

        GUILayout.EndHorizontal();
    }

    public static void RotationXDrawer(ConfigEntryBase entry)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<--", GUILayout.Width(45)))
        {
            ZoneConfigManager.RotationConfigX.Value -= ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustRotation();
        }

        GUILayout.TextField(ZoneConfigManager.RotationConfigX.Value.ToString(), GUILayout.ExpandWidth(true));
        if (GUILayout.Button("-->", GUILayout.Width(45)))
        {
            ZoneConfigManager.RotationConfigX.Value += ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustRotation();
        }

        GUILayout.EndHorizontal();
    }

    public static void RotationYDrawer(ConfigEntryBase entry)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<--", GUILayout.Width(45)))
        {
            ZoneConfigManager.RotationConfigY.Value -= ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustRotation();
        }

        GUILayout.TextField(ZoneConfigManager.RotationConfigY.Value.ToString(), GUILayout.ExpandWidth(true));
        if (GUILayout.Button("-->", GUILayout.Width(45)))
        {
            ZoneConfigManager.RotationConfigY.Value += ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustRotation();
        }

        GUILayout.EndHorizontal();
    }

    public static void RotationZDrawer(ConfigEntryBase entry)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<--", GUILayout.Width(45)))
        {
            ZoneConfigManager.RotationConfigZ.Value -= ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustRotation();
        }

        GUILayout.TextField(ZoneConfigManager.RotationConfigZ.Value.ToString(), GUILayout.ExpandWidth(true));
        if (GUILayout.Button("-->", GUILayout.Width(45)))
        {
            ZoneConfigManager.RotationConfigZ.Value += ZoneConfigManager.ZoneAdjustmentValue.Value;
            ZoneService.AdjustRotation();
        }

        GUILayout.EndHorizontal();
    }

    public static void OutputDrawer(ConfigEntryBase entry)
    {
        if (GUILayout.Button("Output Zones", GUILayout.ExpandWidth(true)))
            ZoneService.OutputZones();
    }
}
