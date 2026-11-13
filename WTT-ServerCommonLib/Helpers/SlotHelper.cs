using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace WTTServerCommonLib.Helpers;

[Injectable]
public class SlotHelper
{
    public void ReplaceSlotFilters(
        TemplateItem item,
        int slotIndex,
        int filterIndex,
        HashSet<MongoId> ids)
    {
        var slot = GetSlotAtIndex(item, slotIndex);
        var filter = GetSlotFilterAtIndex(slot, filterIndex);

        filter.Filter = ids;
    }

    public void ModifySlotFilters(
        TemplateItem item,
        int slotIndex,
        int filterIndex,
        List<MongoId> ids,
        bool isCartridge = false)
    {
        var slot = GetSlotAtIndex(item, slotIndex, isCartridge);
        var filter = GetSlotFilterAtIndex(slot, filterIndex);

        filter.Filter ??= [];
        filter.Filter.UnionWith(ids);
    }

    /// <summary>
    /// Ensures a slot with the given name exists on the item.
    /// If it does not exist, creates it with the provided parameters.
    /// </summary>
    /// <param name="item">Item to search for the slot on.</param>
    /// <param name="slotName">Name of the slot.</param>
    /// <param name="prototype">Prototype ID used when creating the slot.</param>
    /// <param name="required">Whether the new slot is required.</param>
    /// <param name="mergeSlotWithChildren">
    /// Whether the new slot should merge with its children.
    /// </param>
    /// <param name="shift">Shift value for the new slot's first filter.</param>
    /// <returns>The existing or newly created slot.</returns>
    public Slot EnsureSlot(
        TemplateItem item,
        string slotName,
        string prototype,
        bool required = false,
        bool mergeSlotWithChildren = false,
        int shift = 0)
    {
        item.Properties ??= new TemplateItemProperties();

        var slots = item.Properties.Slots?.ToList() ?? [];
        var slot = slots.FirstOrDefault(s => s.Name == slotName);

        if (slot != null)
        {
            return slot;
        }

        slot = new Slot
        {
            Id = new MongoId(),
            Name = slotName,
            Parent = item.Id,
            Required = required,
            MergeSlotWithChildren = mergeSlotWithChildren,
            Prototype = prototype,
            Properties = new SlotProperties
            {
                Filters =
                [
                    new()
                    {
                        Filter = [],
                        Shift = shift
                    }
                ]
            }
        };

        slots.Add(slot);
        item.Properties.Slots = slots;

        return slot;
    }

    /// <summary>
    /// Adds IDs to the slot's first filter.
    /// Creates the slot properties and first filter if needed.
    /// </summary>
    /// <param name="slot">Slot to modify.</param>
    /// <param name="ids">IDs to add.</param>
    public void AddIdsToSlot(Slot slot, params string[] ids)
    {
        slot.Properties ??= new SlotProperties();

        var filters = slot.Properties.Filters?.ToList()
                      ?? [];

        var filterContainer = filters.FirstOrDefault();

        if (filterContainer == null)
        {
            filterContainer = new SlotFilter
            {
                Filter = [],
                Shift = 0
            };

            filters.Add(filterContainer);
            slot.Properties.Filters = filters;
        }

        filterContainer.Filter ??= [];
        filterContainer.Filter.UnionWith(ids.Select(id => (MongoId)id));
    }

    /// <summary>
    /// Adds IDs to the first filter of an existing named slot.
    /// Creates the slot properties and first filter if needed.
    /// Slot-name matching is case-sensitive.
    /// </summary>
    /// <param name="item">Item containing the slot.</param>
    /// <param name="slotName">Name of the slot.</param>
    /// <param name="ids">IDs to add.</param>
    /// <exception cref="InvalidOperationException">
    /// The slot does not exist. Use EnsureSlot to create it first.
    /// </exception>
    public void AddIdsToNamedSlot(
        TemplateItem item,
        string slotName,
        params string[] ids)
    {
        var slot = item.Properties?.Slots?
            .FirstOrDefault(s => s.Name == slotName)
            ?? throw new InvalidOperationException(
                $"Slot '{slotName}' not found on '{item.Name}'");

        AddIdsToSlot(slot, ids);
    }

    /// <summary>
    /// Adds an ID to an existing named slot and filter.
    /// Slot-name matching is case-insensitive; occurrence is zero-based.
    /// </summary>
    public void AddIdToNamedSlot(
        TemplateItem item,
        string slotName,
        string itemId,
        int filterIndex = 0,
        int slotOccurrence = 0)
    {
        EditFilters(
            [item],
            slotName,
            itemId,
            filterIndex,
            slotOccurrence,
            add: true);
    }

    /// <summary>
    /// Removes an ID from an existing named slot and filter.
    /// Slot-name matching is case-insensitive; occurrence is zero-based.
    /// </summary>
    public void RemoveIdFromNamedSlot(
        TemplateItem item,
        string slotName,
        string itemId,
        int filterIndex = 0,
        int slotOccurrence = 0)
    {
        EditFilters(
            [item],
            slotName,
            itemId,
            filterIndex,
            slotOccurrence,
            add: false);
    }

    /// <summary>
    /// Adds an ID to an existing named slot and filter on multiple parents.
    /// Resolves all requested parents, slots, and filters before modifying any.
    /// Unrelated IDs are preserved.
    /// </summary>
    public void AddIdToNamedSlots(
        TemplateTable templates,
        string slotName,
        string itemId,
        IEnumerable<string> parentIds,
        int filterIndex = 0,
        int slotOccurrence = 0)
    {
        EditFilters(
            ResolveParents(templates, parentIds),
            slotName,
            itemId,
            filterIndex,
            slotOccurrence,
            add: true);
    }

    /// <summary>
    /// Removes an ID from an existing named slot and filter on multiple parents.
    /// Resolves all requested parents, slots, and filters before modifying any.
    /// Unrelated IDs are preserved.
    /// </summary>
    public void RemoveIdFromNamedSlots(
        TemplateTable templates,
        string slotName,
        string itemId,
        IEnumerable<string> parentIds,
        int filterIndex = 0,
        int slotOccurrence = 0)
    {
        EditFilters(
            ResolveParents(templates, parentIds),
            slotName,
            itemId,
            filterIndex,
            slotOccurrence,
            add: false);
    }

    private Slot GetSlotAtIndex(
        TemplateItem item,
        int index,
        bool isCartridge = false)
    {
        var slots = isCartridge
            ? item.Properties?.Cartridges?.ToArray()
            : item.Properties?.Slots?.ToArray();

        if (slots != null && index >= 0 && index < slots.Length)
        {
            return slots[index];
        }

        throw new IndexOutOfRangeException(
            $"Index on item slot property `{item.Name}` is out of range");
    }

    private SlotFilter GetSlotFilterAtIndex(Slot slot, int index)
    {
        var filters = slot.Properties?.Filters?.ToArray()
                      ?? [];

        if (index >= 0 && index < filters.Length)
        {
            return filters[index];
        }

        throw new IndexOutOfRangeException(
            $"Index on slot property `{slot.Name}` is out of range");
    }

    private static IEnumerable<TemplateItem> ResolveParents(
        TemplateTable templates,
        IEnumerable<string> parentIds)
    {
        foreach (var parentId in parentIds.Distinct())
        {
            MongoId id = parentId;

            if (!templates.Items.TryGetValue(id, out var item))
            {
                throw new InvalidOperationException(
                    $"Parent item '{parentId}' was not found.");
            }

            yield return item;
        }
    }

    private static void EditFilters(
        IEnumerable<TemplateItem> items,
        string slotName,
        string itemId,
        int filterIndex,
        int slotOccurrence,
        bool add)
    {
        MongoId id = itemId;

        ArgumentOutOfRangeException.ThrowIfNegative(filterIndex);

        ArgumentOutOfRangeException.ThrowIfNegative(slotOccurrence);

        if (string.IsNullOrWhiteSpace(slotName))
        {
            throw new ArgumentException(
                "A slot name is required.",
                nameof(slotName));
        }

        var filters = items.Select(item =>
        {
            var slot = item.Properties?.Slots?
                .Where(s => string.Equals(
                    s.Name,
                    slotName,
                    StringComparison.OrdinalIgnoreCase))
                .Skip(slotOccurrence)
                .FirstOrDefault()
                ?? throw new InvalidOperationException(
                    $"Slot '{slotName}' occurrence {slotOccurrence} " +
                    $"was not found on '{item.Name}' ({item.Id}).");

            return slot.Properties?.Filters?.ElementAtOrDefault(filterIndex)
                ?? throw new InvalidOperationException(
                    $"Filter {filterIndex} in slot '{slotName}' " +
                    $"was not found on '{item.Name}' ({item.Id}).");
        }).ToArray();

        foreach (var filter in filters)
        {
            if (add)
            {
                filter.Filter ??= [];
                filter.Filter.Add(id);
            }
            else
            {
                filter.Filter?.Remove(id);
            }
        }
    }
}
