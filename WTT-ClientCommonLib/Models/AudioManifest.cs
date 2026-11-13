using System.Collections.Generic;

namespace WTTClientCommonLib.Models;

public class AudioManifest
{
    public List<string> AudioBundles { get; set; } = [];
    public Dictionary<string, FaceCardAudioEntry> FaceCardMappings { get; set; } = [];
    public List<string> RadioAudio { get; set; } = [];
}

public class FaceCardAudioEntry
{
    public List<string> Audio { get; set; } = [];
    public bool PlayOnRadio { get; set; } = false;
}
