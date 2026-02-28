namespace Nexticz.Module.Vh.Contracts.Reports;

public class PerformanceReportsRequest
{
    public required string CenterCode { get; set; }
    public required int Year { get; set; }
    public required int Month { get; set; }
}