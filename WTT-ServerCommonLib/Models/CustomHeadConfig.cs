using System.Text.Json.Serialization;

namespace WTTServerCommonLib.Models;

public class CustomHeadConfig
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("addHeadToPlayer")]
    public bool AddHeadToPlayer { get; set; }

    [JsonPropertyName("side")]
    public List<string> Side { get; set; } = new(Array.Empty<string>());

    [JsonPropertyName("locales")]
    public Dictionary<string, string>? Locales { get; set; }

    [JsonPropertyName("availableAsDefault")]
    public bool? AvailableAsDefault { get; set; }
}
