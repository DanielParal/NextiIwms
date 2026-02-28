using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

public record ShiftSummaryResponse(
    [property: Required] Guid Id,
    [property: Required] string Name,
    [property: Required] DateTimeOffset StartDate,
    [property: Required] DateTimeOffset EndDate,
    [property: Required] string Efficiency,
    [property: Required] string AdjustmentTime,
    [property: Required] string DowntimeTime,
    [property: Required] string ShutdownTime,
    [property: Required] ShiftSummaryWashingMachineContract[] WashingMachines);