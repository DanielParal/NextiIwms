using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.LoadedActivities;

public abstract class LoadedActivitiesErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-loadedActivitiesService-";

    public static Error LoadedActivityWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "loadedActivityWithIdDoesnotExist",
        "Načtená aktivita s tímto ID neexistuje");

    public static Error UpdateLoadedActivityError => Error.Validation(
        ComponentSlug + "updateLoadedActivityError",
        "Načtenou aktivitu se nepodařil upravit");
}