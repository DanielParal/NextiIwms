using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Centers.Common.Models;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Contracts.Centers;
using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.Centers.Persistence;

public class CentersRepository(DataContext context) : ICentersRepository
{
    public async Task<Center?> GetCenterByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Centers
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Center?> GetCenterByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await context.Centers
            .Where(x => x.Code == code)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CenterResponse?> GetCenterResponseByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Centers
            .Where(x => x.Id == id)
            .Select(x => new CenterResponse
                { Id = x.Id, Name = x.Name, Code = x.Code, ShowDashboardSalaryData = x.ShowDashboardSalaryData })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetCentersAsync(CentersFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.Centers;

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }

    public async Task<FilteredResult> GetCentersResponseAsync(CentersFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.Centers
            .Select(x => new CenterResponse
                { Id = x.Id, Name = x.Name, Code = x.Code, ShowDashboardSalaryData = x.ShowDashboardSalaryData });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}