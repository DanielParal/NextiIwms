using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Packagings;

public record UpdatePackagingRequest(
    [property: Required] string PackagingTypeCode,
    [property: Required] string PackagingCirculationCode,
    [property: Required] string Name,
    [property: Required] bool MustBeWashed,
    [property: Required] DimensionsContract Dimensions,
    [property: Required] decimal Weight,
    [property: Required] WashingMachineSpeedContract[] WashingMachineSpeeds);