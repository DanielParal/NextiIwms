using Nexticz.Module.Vh.Contracts.DepositorsGroups;

namespace Nexticz.Module.Vh.Contracts.Reports;

public class ReportActivitiesPerformanceResponse
{
    public required List<ReportActivitiesPerformanceDay> ReportWorkerShiftsPerformanceDays { get; set; }
    public required ReportActivitiesPerformanceSum ReportWorkerShiftsPerformanceSum { get; set; }
    public required List<DepositorsGroupResponse> DepositorsGroups { get; set; }
}

public class ReportActivitiesPerformanceDay
{
    public required int Day { get; set; }

    public required List<ReportActivitiesPerformanceDepositorGroupActivities>
        ReportActivitiesPerformanceDepositorGroupsActivities { get; set; }

    public required decimal ActivitiesCount { get; set; }
    public required decimal WorkersFundHours { get; set; }
    public required decimal NonDispensingActivitiesHours { get; set; }
    public required decimal SystemActivitiesHours { get; set; }
    public required decimal PerformanceSum { get; set; }
}

public class ReportActivitiesPerformanceSum
{
    public required List<ReportActivitiesPerformanceDepositorGroupActivities> ReportActivitiesPerformanceDepositorGroups
    {
        get;
        set;
    }

    public required decimal ActivitiesCount { get; set; }
    public required decimal WorkersFundHours { get; set; }
    public required decimal NonDispensingActivitiesHours { get; set; }
    public required decimal SystemActivitiesHours { get; set; }
    public required decimal PerformanceSum { get; set; }
}

public class ReportActivitiesPerformanceDepositorGroupActivities
{
    public required string? DepositorGroupCode { get; set; }
    public required decimal ActivitiesCount { get; set; }
}