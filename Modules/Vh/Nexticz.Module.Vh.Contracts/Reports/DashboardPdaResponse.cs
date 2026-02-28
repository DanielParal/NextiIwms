using Nexticz.Module.Vh.Contracts.WorkerShifts;

namespace Nexticz.Module.Vh.Contracts.Reports;

public class DashboardPdaResponse
{
    public required decimal ScoreSum { get; set; }
    public required decimal ZoneNumber { get; set; }
    public required decimal AverageScore { get; set; }
    public required decimal ActualSalary { get; set; }
    public required bool ShowSalaryData { get; set; }
    public required List<DashboardPdaPaidType> PaidTypes { get; set; }
}

public class DashboardPdaPaidType
{
    public required string Name { get; set; } = "";
    public required ActivityType ActivityType { get; set; }
    public required string Color { get; set; }
    public required List<DashboardPdaActivityType> ActivityTypes { get; set; }
}

public class DashboardPdaActivityType
{
    public required string Name { get; set; } = "";
    public required List<DashboardPdaActivityTypesItem> DashboardPdaActivityTypesItems { get; set; }
    public required decimal ScoreSum { get; set; }
    public required decimal DurationTimeSum { get; set; }
}

public class DashboardPdaActivityTypesItem
{
    public required string Name { get; set; }
    public required decimal Count { get; set; }
    public required string Unit { get; set; }
    public required decimal Coefficient { get; set; }
    public required decimal Score { get; set; }
}