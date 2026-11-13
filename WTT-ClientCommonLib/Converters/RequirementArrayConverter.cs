using System;
using System.Collections.Generic;
using EFT.Hideout;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WTTClientCommonLib.Helpers;

namespace WTTClientCommonLib.Converters;

public class RequirementArrayConverter : JsonConverter
{
    public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var array = JArray.Load(reader);
        var result = new List<Requirement>();

        foreach (var token in array)
        {
            if (token.Type != JTokenType.Object)
            {
                continue;
            }

            var jsonObject = (JObject)token;
            var typeName = jsonObject.Value<string>("type");
            if (typeName == null)
            {
                continue;
            }

            var found = Enum.TryParse(typeName, out ERequirementType requirementType);
            if (!found)
            {
                LogHelper.LogWarn($"Could not convert requirement type {typeName} to ERequirementType");
            }

            Requirement requirement = requirementType switch
            {
                ERequirementType.Area => new AreaRequirement(),
                ERequirementType.BodyPartBuff => new BodyPartBuffRequirement(),
                ERequirementType.GameVersion => new GameVersionRequirement(),
                ERequirementType.Health => new HealthRequirement(),
                ERequirementType.Item => new ItemRequirement(),
                ERequirementType.QuestComplete => new QuestRequirement(),
                ERequirementType.Resource => new ResourceRequirement(),
                ERequirementType.Skill => new SkillRequirement(),
                ERequirementType.Tool => new ToolRequirement(),
                ERequirementType.TraderLoyalty => new TraderLoyaltyRequirement(),
                ERequirementType.TraderUnlock => new TraderUnlockRequirement(),
                _ => null,
            };

            if (requirement == null)
            {
                return null;
            }

            serializer.Populate(jsonObject.CreateReader(), requirement);
            result.Add(requirement);
        }

        return result.ToArray();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Requirement[]);
    }
}
