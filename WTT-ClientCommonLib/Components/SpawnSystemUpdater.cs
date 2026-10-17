using System;
using UnityEngine;
using WTTClientCommonLib.CommandProcessor;
using WTTClientCommonLib.Helpers;

namespace WTTClientCommonLib.Components;

public class SpawnSystemUpdater : MonoBehaviour
{
    private readonly SpawnCommands _spawnCommands;

    public SpawnSystemUpdater()
    {
        _spawnCommands = WTTClientCommonLib.Instance.SpawnCommands;
    }

    private void Update()
    {
        try
        {
            _spawnCommands.UpdateEditMode();

            if (_spawnCommands.IsEditing)
                if (_spawnCommands.LastSpawnedObject != null)
                {
                    var pos = _spawnCommands.LastSpawnedObject.transform.position;
                }
        }
        catch (Exception ex)
        {
            LogHelper.LogError($"Updater failed: {ex}");
        }
    }
}
