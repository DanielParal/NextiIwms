using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

public record WashingStateShiftSummaryContract(
    [property: Required] Guid Id,
    [property: Required] TimeSpan AdjustmentTime,
    [property: Required] TimeSpan ShutdownTime,
    [property: Required] TimeSpan DowntimeTime,
    [property: Required] string Efficiency,
    [property: Required] WashingStateMachineSummaryContract[] MachineSummaries);