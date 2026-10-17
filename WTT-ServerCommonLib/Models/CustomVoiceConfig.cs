using System.Text.Json.Serialization;

namespace WTTServerCommonLib.Models;

public class CustomVoiceConfig
{
    [JsonPropertyName("locales")]
    public Dictionary<string, string>? Locales { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("bundlePath")]
    public string BundlePath { get; set; } = string.Empty;

    [JsonPropertyName("addVoiceToPlayer")]
    public bool AddVoiceToPlayer { get; set; }

    [JsonPropertyName("sideSpecificVoice")]
    public List<string>? SideSpecificVoice { get; set; }

    [JsonPropertyName("addToBotTypes")]
    public Dictionary<string, int>? AddToBotTypes { get; set; }
}
