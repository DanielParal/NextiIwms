using DevExtreme.AspNet.Data;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.ReportActivities.Common.Models;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.ReportActivities.Persistence;

public class ReportActivitiesRepository(DataContext context) : IReportActivitiesRepository
{
    public async Task<FilteredResult> GetReportActivitiesAsync(ReportActivitiesFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.ReportActivities;

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        loadOption.RemoteGrouping = false;
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}