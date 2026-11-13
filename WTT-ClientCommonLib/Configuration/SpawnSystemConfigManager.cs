using BepInEx.Configuration;
using UnityEngine;
using WTTClientCommonLib.Attributes;

namespace WTTClientCommonLib.Configuration;

public static class StaticSpawnSystemConfigManager
{
    internal static ConfigEntry<KeyboardShortcut> MoveForwardKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> MoveBackwardKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> MoveLeftKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> MoveRightKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> MoveUpKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> MoveDownKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> RotatePitchUpKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> RotatePitchDownKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> RotateYawLeftKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> RotateYawRightKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> RotatePitchRollLeftKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> RotatePitchRollRightKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> RotatePitchRollLeftInvertKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> RotatePitchRollRightInvertKey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> DeleteSelectedObject { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> CycleSpawnedObjects { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> CyclePreviousSpawnedObject { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> ConfirmPositionKey { get; private set; }

    public static void Initialize(ConfigFile config)
    {
        if (!UniversalConfigManager.DeveloperMode.Value)
        {
            return;
        }

        CreateKeybindConfigs(config);
    }

    private static void CreateKeybindConfigs(ConfigFile config)
    {
        var orderCounter = 10;
        MoveForwardKey = CreateKeybind(config, "Keybinds", "Move Forward", KeyCode.W, orderCounter++);
        MoveBackwardKey = CreateKeybind(config, "Keybinds", "Move Backward", KeyCode.S, orderCounter++);
        MoveLeftKey = CreateKeybind(config, "Keybinds", "Move Left", KeyCode.A, orderCounter++);
        MoveRightKey = CreateKeybind(config, "Keybinds", "Move Right", KeyCode.D, orderCounter++);
        MoveUpKey = CreateKeybind(config, "Keybinds", "Move Up", KeyCode.E, orderCounter++);
        MoveDownKey = CreateKeybind(config, "Keybinds", "Move Down", KeyCode.Q, orderCounter++);
        RotatePitchUpKey = CreateKeybind(config, "Keybinds", "Rotate Pitch Up", KeyCode.Keypad8, orderCounter++);
        RotatePitchDownKey = CreateKeybind(config, "Keybinds", "Rotate Pitch Down", KeyCode.Keypad2, orderCounter++);
        RotateYawLeftKey = CreateKeybind(config, "Keybinds", "Rotate Yaw Left", KeyCode.Keypad4, orderCounter++);
        RotateYawRightKey = CreateKeybind(config, "Keybinds", "Rotate Yaw Right", KeyCode.Keypad6, orderCounter++);
        RotatePitchRollLeftKey = CreateKeybind(config, "Keybinds", "Rotate Pitch+Roll Left", KeyCode.Keypad7, orderCounter++);
        RotatePitchRollRightKey = CreateKeybind(config, "Keybinds", "Rotate Pitch+Roll Right", KeyCode.Keypad9, orderCounter++);
        RotatePitchRollLeftInvertKey = CreateKeybind(
            config,
            "Keybinds",
            "Rotate Pitch+Roll Left Inverted",
            KeyCode.Keypad1,
            orderCounter++
        );
        RotatePitchRollRightInvertKey = CreateKeybind(
            config,
            "Keybinds",
            "Rotate Pitch+Roll Right Inverted",
            KeyCode.Keypad3,
            orderCounter++
        );
        DeleteSelectedObject = CreateKeybind(config, "Keybinds", "Delete Selected Object", KeyCode.Delete, orderCounter++);
        CycleSpawnedObjects = CreateKeybind(config, "Keybinds", "Cycle Spawned Object", KeyCode.Period, orderCounter++);
        CyclePreviousSpawnedObject = CreateKeybind(config, "Keybinds", "Cycle Previous Spawned Object", KeyCode.Comma, orderCounter++);
        ConfirmPositionKey = CreateKeybind(config, "Keybinds", "Confirm Position", KeyCode.Backslash, orderCounter++);
    }

    private static ConfigEntry<KeyboardShortcut> CreateKeybind(
        ConfigFile config,
        string section,
        string name,
        KeyCode defaultKey,
        int order
    )
    {
        return config.Bind(
            section,
            name,
            new KeyboardShortcut(defaultKey),
            new ConfigDescription($"Key for {name}", null, new ConfigurationManagerAttributes { Order = order })
        );
    }
}
