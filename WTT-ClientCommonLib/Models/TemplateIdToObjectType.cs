using System;
using EFT.InventoryLogic;

public class TemplateIdToObjectType(string templateId, Type itemType, Type templateType, Func<string, object, Item> constructor)
{
    public string TemplateId { get; set; } = templateId;
    public Type ItemType { get; set; } = itemType;
    public Type TemplateType { get; set; } = templateType;
    public Func<string, object, Item> Constructor { get; set; } = constructor;
}
