using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

public record ShiftSummaryWashingMachineContract(
    [property: Required] string Code,
    [property: Required] string Name,
    [property: Required] string Efficiency,
    [property: Required] string AdjustmentTime,
    [property: Required] string DowntimeTime,
    [property: Required] string ShutdownTime);