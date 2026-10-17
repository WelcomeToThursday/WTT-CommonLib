using BepInEx.Configuration;
using UnityEngine;

namespace WTTClientCommonLib.Helpers;

public static class KeyboardShortcutExtensions
{
    public static bool BetterIsPressed(this KeyboardShortcut key)
    {
        if (!Input.GetKey(key.MainKey))
            return false;

        foreach (var modifier in key.Modifiers)
            if (!Input.GetKey(modifier))
                return false;

        return true;
    }

    public static bool BetterIsDown(this KeyboardShortcut key)
    {
        if (!Input.GetKeyDown(key.MainKey))
            return false;

        foreach (var modifier in key.Modifiers)
            if (!Input.GetKey(modifier))
                return false;

        return true;
    }
}
