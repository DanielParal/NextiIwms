using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Drying.Application.Kits;

internal abstract class KitErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-drying-api-kitsService-";
    
    public static Error KitNotFound => Error.NotFound(
        ComponentSlug + "KitNotFound",
        $"Kit nebyl nalezen v sušení."
    );
    
    public static Error ValidationBatchIdIsRequired => Error.Validation(
        ComponentSlug + "ValidationBatchIdIsRequired",
        $"Id dávky je povidnné pole."
    );
    
    public static Error ValidationLineCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationLineCodeIsRequired",
        $"Kód lajny je povinné pole."
    );
    
    public static Error ValidationKitCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationKitCodeIsRequired",
        $"Kód kitu je povinné pole."
    );
    
    public static Error ValidationKitIdIsRequired => Error.Validation(
        ComponentSlug + "ValidationKitIdIsRequired",
        $"Id kitu je povinné pole."
    );
}