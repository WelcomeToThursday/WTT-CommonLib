using System.Collections.Generic;

namespace WTTClientCommonLib.Models;

public class AudioManifest
{
    public List<string> AudioBundles { get; set; } = new();
    public Dictionary<string, FaceCardAudioEntry> FaceCardMappings { get; set; } = new();
    public List<string> RadioAudio { get; set; } = new();
}

public class FaceCardAudioEntry
{
    public List<string> Audio { get; set; } = new();
    public bool PlayOnRadio { get; set; } = false;
}
