using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EFT.InventoryLogic;
using WTTClientCommonLib.Attributes;
using WTTClientCommonLib.Helpers;

namespace WTTClientCommonLib.Services;

public class CustomItemParentLoader
{
    public static CustomItemParentLoader Instance;

    private Dictionary<string, ItemTemplate> _customParents =
        new Dictionary<string, ItemTemplate>();
    private bool _parentsRegistered = false;

    public CustomItemParentLoader()
    {
        if (Instance != null)
            return;

        Instance = this;
    }

    public void FetchCustomParentsFromServer()
    {
        try
        {
            var parentData = Helpers.Utils.Get<Dictionary<string, ItemTemplate>>(
                "/wttcommonlib/parents/custom/get"
            );

            if (parentData != null)
            {
                _customParents = parentData;
            }
            else
            {
                LogHelper.LogError("Failed to fetch custom parent data from server");
            }
        }
        catch (Exception e)
        {
            LogHelper.LogError(e.ToString());
            throw;
        }
    }

    public async Task RegisterCustomParents()
    {
        if (_parentsRegistered)
            return;

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        List<CustomParent> customParents = [];
        List<TemplateIdToObjectType> mappings = [];

        // load all classes that use customparent attribute
        foreach (var assembly in assemblies)
        {
            foreach (var type in GetLoadableTypes(assembly))
            {
                foreach (CustomParent attribute in GetCustomParents(type))
                {
                    customParents.Add(attribute);
                }
            }
        }

        foreach (var parent in customParents)
        {
            if (parent.Item == null)
            {
                mappings.Add(
                    new TemplateIdToObjectType(parent.ParentId, null, parent.Template, null)
                );
                continue;
            }

            var ctor =
                parent.Template == null
                    ? parent.Item.GetConstructor(new[] { typeof(string) })
                    : parent.Item.GetConstructor(new[] { typeof(string), parent.Template });

            if (ctor == null)
            {
                LogHelper.LogError($"Could not find valid constructor for {parent.Item}");
                continue;
            }

            mappings.Add(
                new TemplateIdToObjectType(
                    parent.ParentId,
                    parent.Item,
                    parent.Template,
                    (id, tpl) =>
                        (Item)(
                            parent.Template == null
                                ? ctor.Invoke(new object[] { id })
                                : ctor.Invoke(new object[] { id, tpl })
                        )
                )
            );
        }

        CustomTemplateIdToObjectService.AddNewTemplateIdToObjectMapping(mappings);

        _parentsRegistered = true;
    }

    /// <summary>
    ///     Returns every type in <paramref name="assembly" /> that can actually be loaded.
    /// </summary>
    /// <remarks>
    ///     A single unloadable type — for example a mod whose compile-time dependency has since
    ///     changed shape — makes <see cref="Assembly.GetTypes" /> throw
    ///     <see cref="ReflectionTypeLoadException" /> for the *whole* assembly. Letting that escape
    ///     aborts <see cref="RegisterCustomParents" /> before
    ///     <see cref="CustomTemplateIdToObjectService.AddNewTemplateIdToObjectMapping" /> is ever
    ///     reached, which silently drops every <c>[CustomParent]</c> mapping in the process and
    ///     leaves the client unable to resolve those taxonomy nodes
    ///     ("No C# type for taxonomy node with id ..."). Recovering the loadable subset keeps
    ///     registration working for every well-formed mod.
    /// </remarks>
    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            var recovered = e.Types.OfType<Type>().ToArray();

            LogHelper.LogError(
                $"Failed to load all types from {assembly.GetName().Name} — "
                + $"{recovered.Length} type(s) recovered, {e.LoaderExceptions.Length} loader error(s). "
                + "Continuing with the recovered types."
            );

            return recovered;
        }
    }

    /// <summary>
    ///     Returns the <see cref="CustomParent" /> attributes declared on <paramref name="type" />.
    /// </summary>
    /// <remarks>
    ///     Reading attributes instantiates them, so a type that survived
    ///     <see cref="GetLoadableTypes" /> can still fail here for the same underlying reason — an
    ///     unloadable type behind one of the attribute's own members, or behind a base type walked
    ///     by the <c>inherit</c> flag. Only <see cref="TypeLoadException" /> (the family
    ///     <see cref="ReflectionTypeLoadException" /> belongs to) is caught, and it is logged and
    ///     scoped to the single offending type; any other exception propagates so a genuinely
    ///     different problem is not masked. There is no partial result to recover from here the way
    ///     there is in <see cref="GetLoadableTypes" />: the exception carries the types of the
    ///     attribute's own assembly, not the attributes declared on <paramref name="type" />.
    /// </remarks>
    private static IEnumerable<CustomParent> GetCustomParents(Type type)
    {
        try
        {
            return type.GetCustomAttributes(typeof(CustomParent), true).Cast<CustomParent>();
        }
        catch (TypeLoadException e)
        {
            LogHelper.LogError(
                $"Failed to read [CustomParent] attributes from {type.FullName}: {e.Message} "
                + "Skipping this type."
            );

            return [];
        }
    }
}
