using Nexticz.Lib.Shared.DevExtreme;


namespace Nexticz.Module.Vh.Application.ReportActivities.Common.Models;

public class ReportPerformanceEvaluationFilteringParams : BaseFilteringParams
{
    public string? CenterCode { get; set; }
    public int? FromYear { get; set; }
    public int? FromMonth { get; set; }
}