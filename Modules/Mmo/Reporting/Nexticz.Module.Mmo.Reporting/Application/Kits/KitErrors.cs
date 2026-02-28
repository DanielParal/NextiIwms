using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Reporting.Application.Kits;

internal class KitErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-reporting-api-kitsService-";
    
    public static Error ValidationStartDateBeforeEndDate => Error.Validation(
        ComponentSlug + "ValidationStartDateBeforeEndDate",
        "Začátek kitu nesmí být před koncem kitu."
    );
    
    public static Error ValidationLineItemIsNotKitType => Error.Validation(
        ComponentSlug + "ValidationLineItemIsNotKitType",
        "Aktivita není typu kitu. Nemůžeme změnit časový interval."
    );
}