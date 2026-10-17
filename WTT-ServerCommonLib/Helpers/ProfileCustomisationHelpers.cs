using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;

namespace WTTServerCommonLib.Helpers;

public static class ProfileCustomisationHelpers
{
    /// <summary>Add a single customisation (no duplicates by (Id, Type)).</summary>
    public static bool AddCustomisation(this SptProfile profile, MongoId id, string type, string source = CustomisationSource.DEFAULT)
    {
        if (profile.CustomisationUnlocks != null && profile.CustomisationUnlocks.Exists(c => c.Id == id && c.Type == type))
        {
            return false;
        }

        profile.CustomisationUnlocks?.Add(
            new CustomisationStorage
            {
                Id = id,
                Type = type,
                Source = source,
            }
        );

        return true;
    }

    /// <summary>Add many customisations of the same type (no duplicates).</summary>
    public static int AddCustomisations(this SptProfile profile, IEnumerable<MongoId> ids, string type, string source)
    {
        var added = 0;

        foreach (var id in ids)
        {
            if (profile.AddCustomisation(id, type, source))
            {
                added++;
            }
        }

        return added;
    }

    /// <summary>
    /// Add customisation items to dev profiles only.
    /// (Uses the existing FullProfileExtensions.IsDeveloperAccount())
    /// </summary>
    public static int AddDevOnlyCustomisations(this SptProfile profile, IEnumerable<(MongoId Id, string Type)> items, string source)
    {
        if (!profile.IsDeveloperAccount())
        {
            return 0;
        }

        var added = 0;
        foreach (var (id, type) in items)
        {
            if (profile.AddCustomisation(id, type, source))
            {
                added++;
            }
        }

        return added;
    }
}
