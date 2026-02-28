using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds;

internal abstract class WashingMachineSpeedErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-washing-api-washingMachineSpeedService-";
    
    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationCodeIsRequired",
        "Kód myčky je povinné pole."
    );
    
    public static Error ValidationSpeedMustNotBeNegative => Error.Validation(
        ComponentSlug + "ValidationCodeIsRequired",
        "Rychlost nesmí být záporná."
    );
}