using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.WashingStates;

public record WashingStateMachineContract(
    [property: Required] string Code,
    [property: Required] int AdjustmentMinutes,
    [property: Required] int ShutdownMinutes,
    [property: Required] int DowntimeMinutes,
    [property: Required] string Efficiency,
    [property: Required] bool IsHelpNeeded,
    [property: Required] WashingStateLineContract[] Lines);