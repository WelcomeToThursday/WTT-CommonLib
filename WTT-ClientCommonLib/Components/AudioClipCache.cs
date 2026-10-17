using System.Collections.Generic;
using UnityEngine;
using WTTClientCommonLib.Helpers;

public class AudioClipCache : MonoBehaviour
{
    private Dictionary<string, AudioClip> _clipMap = new();
    private Dictionary<string, float[]> _audioDataMap = new();

    public void CacheAudioClip(string key, AudioClip clip)
    {
        if (!clip)
        {
            LogHelper.LogWarn($"Attempted to cache null clip for key: {key}");
            return;
        }

        clip.hideFlags = HideFlags.DontUnloadUnusedAsset;
        _clipMap[key] = clip;

        float[] audioData = new float[clip.samples * clip.channels];
        clip.GetData(audioData, 0);
        _audioDataMap[key] = audioData;

        LogHelper.LogDebug($"Cached audio clip: {key}, duration: {clip.length}s, data samples: {audioData.Length}");
    }

    public bool TryGetAudioClip(string key, out AudioClip clip)
    {
        if (_clipMap.TryGetValue(key, out clip) && clip != null)
        {
            LogHelper.LogDebug($"Retrieved clip: {key}, duration: {clip.length}s");
            return true;
        }
        return false;
    }

    public void Clear()
    {
        _clipMap.Clear();
        _audioDataMap.Clear();
    }
}
