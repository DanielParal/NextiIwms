using Nexticz.Module.Vh.Contracts.WorkerShifts;

namespace Nexticz.Module.Vh.Contracts.Reports;

public class ReportActivityResponse
{
    public Guid Id { get; set; }
    public required DateOnly Date { get; set; }
    public required Guid WorkerShiftId { get; set; }
    public required DateTime WorkerShiftStart { get; set; }
    public required DateTime WorkerShiftEnd { get; set; }
    public required string WorkerCenterCode { get; set; }
    public required string CenterCode { get; set; }
    public required ActivitySource ActivitySource { get; set; }
    public required string WorkerCode { get; set; }
    public required string ActivityCode { get; set; }
    public string? Note { get; set; }
    public required decimal DurationTime { get; set; }
    public required int ActivitiesCount { get; set; }
    public required decimal Coefficient { get; set; }
    public required decimal Score { get; set; }
    public required decimal WorkerShiftPowerPercentage { get; set; }
}