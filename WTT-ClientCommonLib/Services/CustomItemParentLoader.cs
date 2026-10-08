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
                var attributes = type.GetCustomAttributes(typeof(CustomParent), true);

                foreach (CustomParent attribute in attributes)
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
        catch (Exception e)
        {
            LogHelper.LogError($"Skipping assembly {assembly.GetName().Name}: {e}");

            return [];
        }
    }
}
