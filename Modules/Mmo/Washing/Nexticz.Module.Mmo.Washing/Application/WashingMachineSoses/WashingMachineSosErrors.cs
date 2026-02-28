using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses;

internal abstract class WashingMachineSosErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-washing-api-washingMachineSosService-";
    
    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationCodeIsRequired",
        "Kód myčky je povinné pole."
    );
    
    public static Error ValidationWashingMachineCodeDoesNotExistInSettings => Error.Validation(
        ComponentSlug + "ValidationWashingMachineCodeDoesNotExistInSettings",
        "Kód myčky neexistuje v nastavení."
    );
}