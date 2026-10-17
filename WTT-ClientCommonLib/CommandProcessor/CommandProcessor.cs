using EFT.Console.Core;
using EFT.UI;
using WTTClientCommonLib.Configuration;
using WTTClientCommonLib.Helpers;

namespace WTTClientCommonLib.CommandProcessor;

public class CommandProcessor(PlayerWorldStats playerWorldStats, SpawnCommands spawnCommands)
{
    public void RegisterCommandProcessor()
    {
        ConsoleScreen.Processor.RegisterCommand(
            "clear",
            delegate
            {
                MonoBehaviourSingleton<PreloaderUI>.Instance.Console.Clear();
            }
        );
        if (UniversalConfigManager.DeveloperMode.Value)
        {
            ConsoleScreen.Processor.RegisterCommand("GetPlayerWorldStats", playerWorldStats.GetPlayerWorldStats);
            ConsoleScreen.Processor.RegisterCommand("EnterEditMode", spawnCommands.StartEditMode);
            ConsoleScreen.Processor.RegisterCommand("ExitEditMode", spawnCommands.ExitEditMode);
            ConsoleScreen.Processor.RegisterCommand("ExportSpawnedObjectInfo", spawnCommands.ExportSpawnedObjectsLocations);

            SpawnObjectCommands.SetSpawnCommands(spawnCommands);
            ConsoleScreen.Processor.RegisterCommandGroup<SpawnObjectCommands>();
        }
    }
}

public class SpawnObjectCommands
{
    private static SpawnCommands _spawnCommands;

    public static void SetSpawnCommands(SpawnCommands spawnCommands)
    {
        _spawnCommands = spawnCommands;
    }

    [ConsoleCommand("SpawnObject", "Spawn Static Object using bundle name, prefab name", "<String>, <String>")]
    public static void SpawnObject(string bundleName, string prefabName)
    {
        _spawnCommands?.SpawnObject(bundleName, prefabName);
    }
}
