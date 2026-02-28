using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

public record WashingStateMachineSummaryContract(
    [property: Required] string Code,
    [property: Required] TimeSpan AdjustmentTime,
    [property: Required] TimeSpan ShutdownTime,
    [property: Required] TimeSpan DowntimeTime,
    [property: Required] string Efficiency);