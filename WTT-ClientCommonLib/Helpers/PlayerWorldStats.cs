using BepInEx.Logging;
using EFT;

namespace WTTClientCommonLib.Helpers;

public class PlayerWorldStats(ManualLogSource logger)
{
    public void GetPlayerWorldStats()
    {
        if (WTTClientCommonLib.Player != null)
            LogPlayerStats("Player", WTTClientCommonLib.Player);
        else
            logger.LogError("Player is null. You aren't in raid or hideout.");
    }

    private void LogPlayerStats(string playerType, Player player)
    {
        LogHelper.LogDebug(
            $"{playerType} Position X: {player.Transform.position.x} Y: {player.Transform.position.y} Z: {player.Transform.position.z}"
        );
        LogHelper.LogDebug(
            $"{playerType} Rotation X: {player.gameObject.transform.rotation.eulerAngles.x} Y: {player.gameObject.transform.rotation.eulerAngles.y} Z: {player.gameObject.transform.rotation.eulerAngles.z}"
        );
    }
}
