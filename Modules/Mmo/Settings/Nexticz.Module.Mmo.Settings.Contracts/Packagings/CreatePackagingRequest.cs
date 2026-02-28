using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Packagings;


public record CreatePackagingRequest(
    [property: Required] string PackagingTypeCode,
    [property: Required] string DepositorCode,
    [property: Required] string PackagingCirculationCode,
    [property: Required] string CustomerNumber,
    [property: Required] string Name,
    [property: Required] bool MustBeWashed,
    [property: Required] DimensionsContract Dimensions,
    [property: Required] decimal Weight,
    [property: Required] WashingMachineSpeedContract[] WashingMachineSpeeds);