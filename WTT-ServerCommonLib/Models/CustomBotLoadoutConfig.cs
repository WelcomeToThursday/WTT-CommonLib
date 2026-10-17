using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace WTTServerCommonLib.Models;

public class CustomBotLoadoutConfig
{
    [JsonPropertyName("chances")]
    public ConfigBotChances? Chances { get; set; }

    [JsonPropertyName("inventory")]
    public ConfigBotInventory? Inventory { get; set; }

    [JsonPropertyName("appearance")]
    public ConfigBotAppearance? Appearance { get; set; }
}

public class ConfigBotChances
{
    [JsonPropertyName("equipment")]
    public Dictionary<string, double>? Equipment { get; set; }

    [JsonPropertyName("weaponMods")]
    public Dictionary<string, double>? WeaponMods { get; set; }

    [JsonPropertyName("equipmentMods")]
    public Dictionary<string, double>? EquipmentMods { get; set; }
}

public class ConfigBotInventory
{
    [JsonPropertyName("equipment")]
    public Dictionary<string, Dictionary<string, double>>? Equipment { get; set; }

    [JsonPropertyName("mods")]
    public Dictionary<string, Dictionary<string, string[]>>? Mods { get; set; }

    [JsonPropertyName("Ammo")]
    public Dictionary<string, Dictionary<string, double>>? Ammo { get; set; }
}

public class ConfigBotAppearance
{
    [JsonPropertyName("body")]
    public Dictionary<MongoId, double>? Body { get; set; }

    [JsonPropertyName("feet")]
    public Dictionary<MongoId, double>? Feet { get; set; }

    [JsonPropertyName("hands")]
    public Dictionary<MongoId, double>? Hands { get; set; }

    [JsonPropertyName("head")]
    public Dictionary<MongoId, double>? Head { get; set; }

    [JsonPropertyName("voice")]
    public Dictionary<MongoId, double>? Voice { get; set; }
}
