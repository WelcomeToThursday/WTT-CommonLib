using System;
using System.Linq;
using System.Reflection;
using EFT;
using SPT.Reflection.Patching;
using WTTClientCommonLib.Configuration;
using WTTClientCommonLib.Helpers;
using WTTClientCommonLib.Services;

namespace WTTClientCommonLib.Patches;

internal class OnGameStarted : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GameWorld).GetMethod("OnGameStarted", BindingFlags.Public | BindingFlags.Instance);
    }

    [PatchPostfix]
    private static void PatchPostfix(GameWorld __instance)
    {
        try
        {
            var currentMap = __instance.LocationId;
            var questZones = QuestZones.GetZones();
            if (questZones == null || questZones.Count == 0)
            {
                LogHelper.LogDebug("No zones data loaded; skipping initialization.");
                return;
            }

            var validZones = questZones
                .Where(zone => string.Equals(zone.ZoneLocation, currentMap, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ZoneConfigManager.ExistingQuestZones = validZones;
            QuestZones.CreateZones(validZones);

            var player = __instance.MainPlayer;
            if (player == null)
            {
                LogHelper.LogError("MainPlayer is null in OnGameStarted");
                return;
            }

            var locationID = __instance.LocationId;
            if (player.Profile == null)
            {
                LogHelper.LogError("Player.Profile is null");
                return;
            }

            if (player.Profile.QuestsData == null)
            {
                LogHelper.LogError("Player.Profile.QuestsData is null");
                return;
            }

            if (string.IsNullOrEmpty(locationID))
            {
                LogHelper.LogError("LocationId is null/empty");
                return;
            }

            var instance = WTTClientCommonLib.Instance;
            if (instance == null)
            {
                LogHelper.LogError("Instance is null in OnGameStarted");
                return;
            }

            var loader = instance.AssetLoader;
            if (loader == null)
            {
                LogHelper.LogError("AssetLoader is null in OnGameStarted");
                return;
            }

            var configs = loader.SpawnConfigs;
            if (configs == null || configs.Count == 0)
            {
                LogHelper.LogDebug("No SpawnConfigs in OnGameStarted");
                return;
            }

            LogHelper.LogDebug($"Processing {configs.Count} spawn configs for {locationID}");
            foreach (var config in configs)
                loader.ProcessSpawnConfig(__instance.MainPlayer, config, locationID);
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
    }
}
