using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;
using WTTClientCommonLib.Components;
using WTTClientCommonLib.Configuration;
using WTTClientCommonLib.Helpers;

namespace WTTClientCommonLib.Patches;

internal class BoomboxAudioPatch : ModulePatch
{
    private static readonly System.Random _random = new();

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(AudioArray), nameof(AudioArray.PlayWithOffset));
    }

    [PatchPrefix]
    private static bool Prefix(AudioArray __instance, ref AudioClip sound, out AudioClip __state)
    {
        __state = null;

        try
        {
            if (__instance.gameObject.name != "BoomboxAudio")
                return true;

            var tracker = __instance.gameObject.GetComponent<HideoutRadioStateTracker>();
            if (tracker == null)
            {
                tracker = __instance.gameObject.AddComponent<HideoutRadioStateTracker>();
            }

            if (ShouldSkipPlayback(tracker))
                return true;

            if (!TryGetAudioResource(tracker, out var audioResource))
                return true;

            if (!ResourceLoader.ClipCache.TryGetAudioClip(audioResource, out var customClip))
            {
                LogHelper.LogWarn($"Audio clip not found in cache: {audioResource}");
                return true;
            }

            __state = sound;
            sound = customClip;
            tracker.HasPlayedFirstEntranceAudio = true;

            float volume = GetVolumeForLocation(tracker.Location);
            __instance.GetComponent<AudioSource>().volume = volume;

#if DEBUG
            LogHelper.LogDebug($"Playing custom audio: {audioResource} at volume {volume} for {tracker.Location}");
#endif

            return true;
        }
        catch (Exception ex)
        {
#if DEBUG
            LogHelper.LogError($"Prefix error: {ex}");
#endif
            return true;
        }
    }

    [PatchPostfix]
    private static void Postfix(AudioArray __instance, AudioClip sound, AudioClip __state)
    {
        if (__state == null || sound == __state)
            return;

        try
        {
            var tracker = __instance.gameObject.GetComponent<HideoutRadioStateTracker>();
            if (tracker == null)
                return;

            var sources = __instance.GetComponents<AudioSource>();
            foreach (var source in sources)
            {
                if (source.clip == sound)
                {
                    var revert = source.gameObject.AddComponent<RevertClipComponent>();
                    revert.Initialize(source, __state, () => { });
                }
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            LogHelper.LogError($"Postfix error: {ex}");
#endif
        }
    }

    private static float GetVolumeForLocation(RadioLocation location)
    {
        return location switch
        {
            RadioLocation.Gym => RadioSettings.GymRadioVolume.Value,
            RadioLocation.RestSpace => RadioSettings.RestSpaceRadioVolume.Value,
            _ => 1f,
        };
    }

    private static bool ShouldSkipPlayback(HideoutRadioStateTracker tracker)
    {
        if (!tracker.Enabled)
        {
#if DEBUG
            LogHelper.LogDebug($"Skipping playback for {tracker.Location} - radio disabled.");
#endif
            return true;
        }

        if (tracker.PlayOnFirstEntrance && !tracker.HasPlayedFirstEntranceAudio)
        {
#if DEBUG
            LogHelper.LogDebug($"Playing first entrance track for {tracker.Location}");
#endif
            return false;
        }

        if (_random.Next(0, 100) > tracker.ReplacementChance)
        {
#if DEBUG
            LogHelper.LogDebug($"Random chance prevented replacement for {tracker.Location}");
#endif
            return true;
        }

        return false;
    }

    private static bool TryGetAudioResource(HideoutRadioStateTracker tracker, out string audioResource)
    {
        audioResource = null;
        var player = GamePlayerOwner.MyPlayer;

        // 1. On first entrance, try to play face-specific audio
        if (tracker.PlayOnFirstEntrance && !tracker.HasPlayedFirstEntranceAudio && player?.Profile != null)
        {
            var faceId = player.Profile.Customization.TryGetValue(EBodyModelPart.Head, out var headId) ? headId : null;
            var faceName = faceId.LocalizedName();
            if (!string.IsNullOrEmpty(faceName))
            {
                var charAudio = ResourceLoader.GetAudioForFace(faceName);
                if (!string.IsNullOrEmpty(charAudio))
                {
                    audioResource = charAudio;
                    return true;
                }
            }
        }

        // 2. RADIO: Build pool from radio + optionally head audio if alsoRadio == true
        List<string> radioPool = new(ResourceLoader.GetRadioAudio());

        if (player?.Profile != null)
        {
            var faceId = player.Profile.Customization.TryGetValue(EBodyModelPart.Head, out var headId) ? headId : null;
            var faceName = faceId.LocalizedName();

            if (
                !string.IsNullOrEmpty(faceName)
                && ResourceLoader.Manifest.FaceCardMappings.TryGetValue(faceName, out var faceEntry)
                && faceEntry.PlayOnRadio
                && faceEntry.Audio is { Count: > 0 }
            )
            {
                // Check if none of the audios are already in the radioPool
                if (!faceEntry.Audio.Any(audioKey => radioPool.Contains(audioKey)))
                {
                    radioPool.AddRange(faceEntry.Audio);
                }
            }
        }

        if (radioPool.Count > 0)
        {
            audioResource = radioPool[UnityEngine.Random.Range(0, radioPool.Count)];
            return true;
        }

        return false;
    }
}
