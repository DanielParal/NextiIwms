using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Packagings;

public record PackagingResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code,
    [property: Required] string PackagingTypeCode,
    [property: Required] string DepositorCode,
    [property: Required] string PackagingCirculationCode,
    [property: Required] string CustomerNumber,
    [property: Required] string Name,
    [property: Required] bool MustBeWashed,
    [property: Required] DimensionsContract Dimensions,
    [property: Required] decimal Weight,
    [property: Required] WashingMachineSpeedContract[] WashingMachineSpeeds
);