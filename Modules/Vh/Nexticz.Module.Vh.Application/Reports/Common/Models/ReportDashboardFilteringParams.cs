using Nexticz.Lib.Shared.DevExtreme;


namespace Nexticz.Module.Vh.Application.ReportActivities.Common.Models;

public class ReportDashboardFilteringParams : BaseFilteringParams
{
    public required string CenterCode { get; set; }
}