namespace Nexticz.Module.Vh.Contracts.Reports;

public class DashboardResponse
{
    public required List<DashboardRow> DashboardRows { get; set; }
    public required TimeOnly DashboardGeneratedTime { get; set; }
    public required bool ShowSalaryData { get; set; }
}

public class DashboardRow
{
    public required string WorkerName { get; set; }
    public required TimeOnly WorkerShiftStart { get; set; }
    public required decimal UnknownActivityTime { get; set; }
    public required decimal MyStockScore { get; set; }
    public required decimal DynamicsScore { get; set; }
    public required decimal IwmsScore { get; set; }
    public required decimal NonProductiveScore { get; set; }
    public required decimal ScoreSum { get; set; }
    public required string TimeSum { get; set; }
    public required decimal AverageScore { get; set; }
    public required decimal ZoneNumber { get; set; }
    public required decimal ZoneReward { get; set; }
    public required decimal ActualSalary { get; set; }
}