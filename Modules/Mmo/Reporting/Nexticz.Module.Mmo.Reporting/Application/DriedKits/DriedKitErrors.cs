using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Reporting.Application.DriedKits;

internal class DriedKitErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-reporting-api-driedKitsService-";
    
    public static Error DriedKitNotFound => Error.NotFound(
        ComponentSlug + "DriedKitNotFound",
        $"Kit nebyl nalezen v průbehu sušení."
    );
    
    public static Error ValidationDriedKitAlreadyExist => Error.Validation(
        ComponentSlug + "ValidationDriedKitAlreadyExist",
        $"Kit již existuje v průběhu praní."
    );
    
    public static Error ValidationDriedKitDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationDriedKitDoesNotExist",
        $"Kit neexistuje v průběhu praní."
    );
    
    public static Error ValidationKitIdIsRequired => Error.Validation(
        ComponentSlug + "ValidationKitIdIsRequired",
        $"Id kit je povinné pole."
    );
    
    public static Error ValidationCompletedKitsCountGreaterThanZero => Error.Validation(
        ComponentSlug + "ValidationCompletedKitsCountGreaterThanZero",
        $"Počet hotových kitů musí bých větší než 0."
    );
    
    public static Error ValidationBatchIdIsRequired => Error.Validation(
        ComponentSlug + "ValidationBatchIdIsRequired",
        $"Id dávky je povinné pole."
    );
    
    public static Error ValidationLineCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationLineCodeIsRequired",
        $"Kód lajny je povinné pole."
    );
    
    public static Error ValidationKitCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationLineCodeIsRequired",
        $"Kód kitu je povinné pole."
    );
    
    public static Error ValidationOptimalDryingTimeGreaterThanZero => Error.Validation(
        ComponentSlug + "ValidationOptimalDryingTimeGreaterThanZero",
        $"Čas schnutí musí bých větší než 0."
    );
    
    public static Error ValidationDryingEndedHasToBeAfterDryingStarted => Error.Validation(
        ComponentSlug + "ValidationDryingEndedHasToBeAfterDryingStarted",
        $"Konec sušení musí nastat po začátku sušení."
    );
}