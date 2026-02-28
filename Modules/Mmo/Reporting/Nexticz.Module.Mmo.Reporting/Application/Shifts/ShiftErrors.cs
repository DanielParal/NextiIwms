using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts;

internal abstract class ShiftErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-reporting-api-shiftsService-";
    
    public static Error ShiftNotFound => Error.NotFound(
        ComponentSlug + "ShiftNotFound",
        $"Směna nebyla nalezena."
    );
    
    public static Error ShiftSettingsNotFound => Error.NotFound(
        ComponentSlug + "ShiftSettingsNotFound",
        $"Nastavení směn nebylo nalezeno."
    );
    
    public static Error FailureDeserializeShiftSettings => Error.Failure(
        ComponentSlug + "FailureDeserializeShiftSettings",
        $"Nemůžeme deserializovat nastavení směn. Zkontrolujte nastavení směn."
    );
    
    public static Error FailureInvalidShiftSettings => Error.Failure(
        ComponentSlug + "FailureInvalidShiftSettings",
        $"Směny jsou špatně nastaveny v nastavení. Prosím kontaktujte administrátora."
    );
    
    public static Error ValidationThereArePendingItemsForReview => Error.Validation(
        ComponentSlug + "ValidationThereArePendingItemsForReview",
        "Nejsou okomentovány všechny potřebné aktivity ve směně."
    );
}