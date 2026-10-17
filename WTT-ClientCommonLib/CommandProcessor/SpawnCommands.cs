using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using Newtonsoft.Json;
using UnityEngine;
using WTTClientCommonLib.Configuration;
using WTTClientCommonLib.Helpers;
using WTTClientCommonLib.Models;
using WTTClientCommonLib.Services;
using Object = UnityEngine.Object;

namespace WTTClientCommonLib.CommandProcessor;

public class SpawnCommands(ManualLogSource logger, AssetLoader assetLoader)
{
    private const float MoveSpeed = 1f;
    private const float RotateSpeed = 15f;
    private bool _altPressed;
    private int _currentSpawnedIndex = -1;
    private GameObject _editIndicator;
    private GamePlayerOwner _gamePlayerOwner;
    private GameObject _lastSpawnedObject;
    private bool _shiftPressed;
    private readonly List<SpawnedObjectInfo> _spawnedObjects = new();

    public GameObject LastSpawnedObject => _currentSpawnedIndex >= 0 ? _spawnedObjects[_currentSpawnedIndex].Object : null;

    public bool IsEditing { get; private set; }

    public void SpawnObject(string bundleName, string prefabName, string[] args = null)
    {
        // Get player position
        var player = Singleton<GameWorld>.Instance.MainPlayer;
        if (player == null)
        {
            LogHelper.LogDebug("Player not found. Are you in a raid?");
            return;
        }

        // Calculate spawn position 2 meters in front of player
        var spawnPosition = player.Transform.position + player.Transform.forward * 3f;
        var spawnRotation = Quaternion.identity;

        // Parse coordinates if provided
        if (args is { Length: >= 3 })
        {
            if (float.TryParse(args[0], out var x) && float.TryParse(args[1], out var y) && float.TryParse(args[2], out var z))
                spawnPosition = new Vector3(x, y, z);

            // Parse rotation if provided
            if (args.Length >= 6)
                if (float.TryParse(args[3], out var rx) && float.TryParse(args[4], out var ry) && float.TryParse(args[5], out var rz))
                    spawnRotation = Quaternion.Euler(rx, ry, rz);
        }

        // Load and spawn the prefab
        var prefab = assetLoader.LoadPrefabFromBundle(bundleName, prefabName);
        if (prefab == null)
        {
            LogHelper.LogDebug($"Failed to load prefab: {prefabName} from bundle: {bundleName}");
            return;
        }

        var newObject = Object.Instantiate(prefab, spawnPosition, spawnRotation);
        _lastSpawnedObject = newObject;

        // Store metadata with the object
        _spawnedObjects.Add(
            new SpawnedObjectInfo
            {
                Object = newObject,
                BundleName = bundleName,
                PrefabName = prefabName,
            }
        );
        _currentSpawnedIndex = _spawnedObjects.Count - 1;
        LogHelper.LogDebug($"Spawned {prefabName} at {spawnPosition}");
    }

    public void StartEditMode()
    {
        if (_lastSpawnedObject == null)
        {
            LogHelper.LogDebug("No object to edit. Spawn an object first.");
            return;
        }

        IsEditing = true;
        _gamePlayerOwner = WTTClientCommonLib.Player.GetComponentInChildren<GamePlayerOwner>();
        _gamePlayerOwner.enabled = false;

        // In StartEditMode():
        if (_editIndicator == null)
        {
            _editIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _editIndicator.transform.localScale = Vector3.one * 0.3f;
            _editIndicator.GetComponent<Renderer>().material.color = Color.red;
            _editIndicator.GetComponent<Collider>().enabled = false;

            // Add visible rotation indicator
            var arrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arrow.transform.SetParent(_editIndicator.transform);
            arrow.transform.localPosition = new Vector3(0, 0, 0.5f);
            arrow.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        }

        LogHelper.LogDebug("Entering edit mode. Use WASD to move, Arrow Keys to rotate. Press Enter to confirm.");
        LogHelper.LogDebug("Current position: " + _lastSpawnedObject.transform.position);
    }

    public void ExitEditMode()
    {
        LogHelper.LogDebug("Exiting edit mode.");
        IsEditing = false;
        _gamePlayerOwner = WTTClientCommonLib.Player.GetComponentInChildren<GamePlayerOwner>();
        _gamePlayerOwner.enabled = true;

        // Clean up visual indicator
        if (_editIndicator != null)
        {
            Object.Destroy(_editIndicator);
            _editIndicator = null;
        }
    }

    public void UpdateEditMode()
    {
        if (!IsEditing || _lastSpawnedObject == null)
        {
            if (IsEditing)
                logger.LogError("EditMode active but no object!");
            return;
        }

        var cameraTransform = Camera.main?.transform;
        if (cameraTransform == null)
            return;

        // Handle object cycling and deletion
        if (StaticSpawnSystemConfigManager.CycleSpawnedObjects.Value.BetterIsDown())
        {
            CycleSpawnedObjects();
        }
        else if (StaticSpawnSystemConfigManager.CyclePreviousSpawnedObject.Value.BetterIsDown())
        {
            CyclePreviousSpawnedObject();
        }
        else if (StaticSpawnSystemConfigManager.DeleteSelectedObject.Value.BetterIsDown())
        {
            DeleteSelectedObject();
            // Skip movement if we just deleted the object
            if (_lastSpawnedObject == null)
                return;
        }

        _shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        _altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

        // Modifier key-based speed multiplier
        var speedMultiplier = 1f;
        if (_shiftPressed)
        {
            speedMultiplier = 2f;
            if (_altPressed)
                speedMultiplier = 4f;
        }

        // Get input states
        var forwardPressed = StaticSpawnSystemConfigManager.MoveForwardKey.Value.BetterIsPressed();
        var backwardPressed = StaticSpawnSystemConfigManager.MoveBackwardKey.Value.BetterIsPressed();
        var leftPressed = StaticSpawnSystemConfigManager.MoveLeftKey.Value.BetterIsPressed();
        var rightPressed = StaticSpawnSystemConfigManager.MoveRightKey.Value.BetterIsPressed();
        var upPressed = StaticSpawnSystemConfigManager.MoveUpKey.Value.BetterIsPressed();
        var downPressed = StaticSpawnSystemConfigManager.MoveDownKey.Value.BetterIsPressed();

        // Camera-relative movement vectors
        var moveDirection = Vector3.zero;
        var forward = cameraTransform.forward;
        var right = cameraTransform.right;
        var up = Vector3.up; // Use world up for vertical movement

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        if (forwardPressed)
            moveDirection += forward;
        if (backwardPressed)
            moveDirection -= forward;
        if (leftPressed)
            moveDirection -= right;
        if (rightPressed)
            moveDirection += right;
        if (upPressed)
            moveDirection += up;
        if (downPressed)
            moveDirection -= up;

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        _lastSpawnedObject.transform.position += moveDirection * (MoveSpeed * speedMultiplier * Time.deltaTime);

        // ROTATION SYSTEM - CORRECTED DIRECTIONS
        var rotationInput = Vector3.zero;

        // Primary directions
        var pitchUpPressed = Input.GetKey(StaticSpawnSystemConfigManager.RotatePitchUpKey.Value.MainKey) && !_shiftPressed && !_altPressed;
        var pitchDownPressed =
            Input.GetKey(StaticSpawnSystemConfigManager.RotatePitchDownKey.Value.MainKey) && !_shiftPressed && !_altPressed;
        var yawLeftPressed = Input.GetKey(StaticSpawnSystemConfigManager.RotateYawLeftKey.Value.MainKey) && !_shiftPressed && !_altPressed;
        var yawRightPressed =
            Input.GetKey(StaticSpawnSystemConfigManager.RotateYawRightKey.Value.MainKey) && !_shiftPressed && !_altPressed;

        if (pitchUpPressed) // 8 - Pitch up (forward)
            rotationInput.x += 1;
        if (pitchDownPressed) // 2 - Pitch down (backward)
            rotationInput.x = 1;
        if (yawLeftPressed) // 4 - Yaw left (counter-clockwise)
            rotationInput.y -= 1;
        if (yawRightPressed) // 6 - Yaw right (clockwise)
            rotationInput.y += 1;

        // Diagonal directions
        var pitchRollLeftKey =
            Input.GetKey(StaticSpawnSystemConfigManager.RotatePitchRollLeftKey.Value.MainKey) && !_shiftPressed && !_altPressed;
        var pitchRollRightKey =
            Input.GetKey(StaticSpawnSystemConfigManager.RotatePitchRollRightKey.Value.MainKey) && !_shiftPressed && !_altPressed;
        var pitchRollLeftInvertKey =
            Input.GetKey(StaticSpawnSystemConfigManager.RotatePitchRollLeftInvertKey.Value.MainKey) && !_shiftPressed && !_altPressed;
        var pitchRollRightInvertKey =
            Input.GetKey(StaticSpawnSystemConfigManager.RotatePitchRollRightInvertKey.Value.MainKey) && !_shiftPressed && !_altPressed;

        if (pitchRollLeftKey) // 7 - Forward left
        {
            rotationInput.x -= 1; // Pitch up
            rotationInput.z += 1; // Roll left
        }

        if (pitchRollRightKey) // 9 - Forward right
        {
            rotationInput.x -= 1; // Pitch up
            rotationInput.z -= 1; // Roll right
        }

        if (pitchRollLeftInvertKey) // 1 - Backward left
        {
            rotationInput.x += 1; // Pitch down
            rotationInput.z += 1; // Roll left
        }

        if (pitchRollRightInvertKey) // 3 - Backward right
        {
            rotationInput.x += 1; // Pitch down
            rotationInput.z -= 1; // Roll right
        }

        if (rotationInput != Vector3.zero)
            // Apply rotation in world space
            _lastSpawnedObject.transform.Rotate(
                rotationInput.x * RotateSpeed * speedMultiplier * Time.deltaTime,
                rotationInput.y * RotateSpeed * speedMultiplier * Time.deltaTime,
                rotationInput.z * RotateSpeed * speedMultiplier * Time.deltaTime,
                Space.World
            );

        if (StaticSpawnSystemConfigManager.ConfirmPositionKey.Value.BetterIsDown())
        {
            ExitEditMode();
            LogHelper.LogDebug("Position confirmed!");
            LogHelper.LogDebug($"Position: {_lastSpawnedObject.transform.position}");
            LogHelper.LogDebug($"Rotation: {_lastSpawnedObject.transform.rotation.eulerAngles}");
        }

        if (_editIndicator != null)
        {
            _editIndicator.transform.position = _lastSpawnedObject.transform.position;
            _editIndicator.transform.rotation = Quaternion.identity; // Keep indicator upright
        }
    }

    private void DeleteSelectedObject()
    {
        if (_spawnedObjects.Count == 0 || _currentSpawnedIndex < 0)
        {
            LogHelper.LogDebug("No object to delete.");
            return;
        }

        var currentInfo = _spawnedObjects[_currentSpawnedIndex];
        Object.Destroy(currentInfo.Object);
        _spawnedObjects.RemoveAt(_currentSpawnedIndex);

        LogHelper.LogDebug($"Deleted object: {currentInfo.PrefabName}");

        // Update selection
        if (_spawnedObjects.Count > 0)
        {
            _currentSpawnedIndex = Mathf.Clamp(_currentSpawnedIndex, 0, _spawnedObjects.Count - 1);
            _lastSpawnedObject = _spawnedObjects[_currentSpawnedIndex].Object;
        }
        else
        {
            _currentSpawnedIndex = -1;
            _lastSpawnedObject = null;

            // Exit edit mode if no objects left
            if (IsEditing)
                ExitEditMode();
        }
    }

    private void CycleSpawnedObjects()
    {
        if (_spawnedObjects.Count == 0)
        {
            LogHelper.LogDebug("No spawned objects to cycle.");
            return;
        }

        _currentSpawnedIndex = (_currentSpawnedIndex + 1) % _spawnedObjects.Count;
        var currentInfo = _spawnedObjects[_currentSpawnedIndex];
        _lastSpawnedObject = currentInfo.Object;

        LogHelper.LogDebug(
            $"Switched to object #{_currentSpawnedIndex}: " + $"{currentInfo.PrefabName} [Bundle: {currentInfo.BundleName}]"
        );

        if (IsEditing && _editIndicator != null)
            _editIndicator.transform.position = _lastSpawnedObject.transform.position;
    }

    private void CyclePreviousSpawnedObject()
    {
        if (_spawnedObjects.Count == 0)
        {
            LogHelper.LogDebug("No spawned objects to cycle.");
            return;
        }

        _currentSpawnedIndex = (_currentSpawnedIndex - 1 + _spawnedObjects.Count) % _spawnedObjects.Count;
        var currentInfo = _spawnedObjects[_currentSpawnedIndex];
        _lastSpawnedObject = currentInfo.Object;

        LogHelper.LogDebug(
            $"Switched to object #{_currentSpawnedIndex}: " + $"{currentInfo.PrefabName} [Bundle: {currentInfo.BundleName}]"
        );

        if (IsEditing && _editIndicator != null)
            _editIndicator.transform.position = _lastSpawnedObject.transform.position;
    }

    public void ExportSpawnedObjectsLocations()
    {
        if (_spawnedObjects == null || _spawnedObjects.Count == 0)
        {
            LogHelper.LogDebug("No spawned objects to export.");
            return;
        }

        var exportList = new List<CustomSpawnConfig>();

        foreach (var info in _spawnedObjects)
        {
            var config = new CustomSpawnConfig
            {
                QuestId = null,
                LocationID = WTTClientCommonLib.Player.Location ?? "unknown_location",
                BundleName = info.BundleName, // Directly use stored name
                PrefabName = info.PrefabName, // Directly use stored name
                Position = info.Object.transform.position,
                Rotation = info.Object.transform.rotation.eulerAngles,
                RequiredQuestStatuses = new List<string>(),
                ExcludedQuestStatuses = new List<string>(),
                QuestMustExist = null,
                LinkedQuestId = null,
                LinkedRequiredStatuses = new List<string>(),
                LinkedExcludedStatuses = new List<string>(),
                LinkedQuestMustExist = null,
                RequiredItemInInventory = null,
                RequiredLevel = null,
                RequiredFaction = null,
                RequiredBossSpawned = null,
            };

            exportList.Add(config);
        }

        var outputDir = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, @"..\..\..\..\"));
        var path = Path.Combine(outputDir, $"WTT-ClientCommonLib-CustomStaticSpawnConfig-Output-{DateTime.Now:yyyyMMddHHmmssffff}.json");
        File.WriteAllText(path, JsonConvert.SerializeObject(exportList, Formatting.Indented));

        LogHelper.LogDebug($"Exported {_spawnedObjects.Count} spawned objects to {path}");
    }
}
