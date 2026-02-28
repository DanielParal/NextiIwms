using Nexticz.Module.Vh.Contracts.DepositorsGroups;

namespace Nexticz.Module.Vh.Contracts.Reports;

public class ReportWorkerShiftsPerformanceResponse
{
    public required List<ReportWorkerShiftsPerformanceDay> ReportWorkerShiftsPerformanceDays { get; set; }
    public required ReportWorkerShiftsPerformanceSum ReportWorkerShiftsPerformanceSum { get; set; }
    public required List<DepositorsGroupResponse> DepositorsGroups { get; set; }
}

public class ReportWorkerShiftsPerformanceDay
{
    public required int Day { get; set; }

    public required List<ReportActivitiesPerformanceDepositorGroup> ReportActivitiesPerformanceDepositorGroups
    {
        get;
        set;
    }

    public required decimal DurationTimeSum { get; set; }
    public required decimal ScoreSum { get; set; }
    public required decimal PerformanceSum { get; set; }
}

public class ReportWorkerShiftsPerformanceSum
{
    public required List<ReportActivitiesPerformanceDepositorGroup> ReportActivitiesPerformanceDepositorGroups
    {
        get;
        set;
    }

    public required decimal DurationTimeSum { get; set; }
    public required decimal ScoreSum { get; set; }
    public required decimal PerformanceSum { get; set; }
}

public class ReportActivitiesPerformanceDepositorGroup
{
    public required string? DepositorGroupCode { get; set; }
    public required decimal DurationTime { get; set; }
    public required decimal Score { get; set; }
    public required decimal Performance { get; set; }
}