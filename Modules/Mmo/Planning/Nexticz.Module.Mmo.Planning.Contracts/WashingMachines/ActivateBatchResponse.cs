using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record ActivateBatchResponse(
    int? CurrentSpeed,
    [property: Required] int RequestedSpeed,
    [property: Required] SpeedLevelContract RequestedSpeedLevel);