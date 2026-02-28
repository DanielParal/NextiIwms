using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.ReportActivities.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IReportActivitiesRepository
{
    Task<FilteredResult> GetReportActivitiesAsync(ReportActivitiesFilteringParams filteringParams,
        CancellationToken cancellationToken);
}