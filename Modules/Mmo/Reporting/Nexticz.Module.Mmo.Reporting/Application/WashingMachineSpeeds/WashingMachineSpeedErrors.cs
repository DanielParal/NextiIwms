using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Reporting.Application.WashingMachineSpeeds;

internal abstract class WashingMachineSpeedErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-reporting-api-washingMachineSpeedService-";
    
    public static Error SpeedNotFound => Error.NotFound(
        ComponentSlug + "SpeedNotFound",
        "Současná rychlost nebyla nalezena."
    );
}