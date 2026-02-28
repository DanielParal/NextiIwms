using DevExtreme.AspNet.Data;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.ReportActivities.Common.Models;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.ReportPerformanceEvaluations.Persistence;

public class ReportPerformanceEvaluationRepository(DataContext context) : IReportPerformanceEvaluationRepository
{
    public async Task<FilteredResult> GetReportPerformanceEvaluationAsync(
        ReportPerformanceEvaluationFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.ReportPerformanceEvaluations
            .Where(x =>
                filteringParams.CenterCode == null ||
                x.CenterCode == filteringParams.CenterCode)
            .Where(x =>
                filteringParams.FromYear == null ||
                x.Date.Year == filteringParams.FromYear)
            .Where(x =>
                filteringParams.FromMonth == null ||
                x.Date.Month == filteringParams.FromMonth);

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}