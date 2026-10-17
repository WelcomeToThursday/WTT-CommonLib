using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace WTTClientCommonLib.Models;

public class CustomSpawnConfig
{
    // Basic spawn information
    [JsonProperty("questId")]
    public string QuestId { get; set; }

    [JsonProperty("locationID")]
    public string LocationID { get; set; }

    [JsonProperty("bundleName")]
    public string BundleName { get; set; }

    [JsonProperty("prefabName")]
    public string PrefabName { get; set; }

    [JsonProperty("position")]
    public Vector3 Position { get; set; } = Vector3.zero;

    [JsonProperty("rotation")]
    public Vector3 Rotation { get; set; } = Vector3.zero;

    [JsonProperty("requiredQuestStatuses")]
    public List<string> RequiredQuestStatuses { get; set; } = new();

    [JsonProperty("excludedQuestStatuses")]
    public List<string> ExcludedQuestStatuses { get; set; } = new();

    [JsonProperty("questMustExist")]
    public bool? QuestMustExist { get; set; }

    [JsonProperty("linkedQuestId")]
    public string LinkedQuestId { get; set; }

    [JsonProperty("linkedRequiredStatuses")]
    public List<string> LinkedRequiredStatuses { get; set; } = new();

    [JsonProperty("linkedExcludedStatuses")]
    public List<string> LinkedExcludedStatuses { get; set; } = new();

    [JsonProperty("linkedQuestMustExist")]
    public bool? LinkedQuestMustExist { get; set; }

    [JsonProperty("requiredItemInInventory")]
    public string RequiredItemInInventory { get; set; }

    [JsonProperty("requiredLevel")]
    public int? RequiredLevel { get; set; }

    [JsonProperty("requiredFaction")]
    public string RequiredFaction { get; set; }

    [JsonProperty("requiredBossSpawned")]
    public string RequiredBossSpawned { get; set; }
}
