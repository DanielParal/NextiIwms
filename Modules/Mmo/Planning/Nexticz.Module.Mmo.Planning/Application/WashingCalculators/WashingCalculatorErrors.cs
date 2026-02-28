using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Planning.Application.WashingCalculators;

internal abstract class WashingCalculatorErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-planning-api-washingCalculator-";
    
    public static Error ValidationPackagingWidth => Error.Validation(
        ComponentSlug + "ValidationPackagingWidth",
        "Šířka obalu musí být větší než 0 pro kalkulaci optimálního času praní."
    );
    
    public static Error ValidationPackagingCount => Error.Validation(
        ComponentSlug + "ValidationPackagingCount",
        "Počet obalů musí být větší než 0 pro kalkulaci optimálního času praní."
    );
    
    public static Error ValidationWashingMachineLength => Error.Validation(
        ComponentSlug + "ValidationWashingMachineLength",
        "Délka myčky musí být větší než 0 pro kalkulaci optimálního času praní."
    );
    
    public static Error ValidationWashingMachineSpeed => Error.Validation(
        ComponentSlug + "ValidationWashingMachineSpeed",
        "Rychlost myčky musí být větší než 0 pro kalkulaci optimálního času praní."
    );
    
    public static Error ValidationPackagingWidthGraterThanWashingMachineLength => Error.Validation(
        ComponentSlug + "ValidationPackagingWidthGraterThanWashingMachineLength",
        "Šířka obalu nesmí být bětší než délka celé myčky pro kalkulaci optimálního času praní."
    );
}