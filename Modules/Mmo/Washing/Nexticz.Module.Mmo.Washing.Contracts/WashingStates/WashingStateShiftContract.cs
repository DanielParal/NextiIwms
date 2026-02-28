using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.WashingStates;

public record WashingStateShiftContract(
    [property: Required] int AdjustmentMinutes,
    [property: Required] int ShutdownMinutes,
    [property: Required] int DowntimeMinutes,
    [property: Required] string Efficiency);