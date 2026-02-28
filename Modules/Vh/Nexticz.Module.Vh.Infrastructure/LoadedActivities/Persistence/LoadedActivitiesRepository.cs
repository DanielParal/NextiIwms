using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.LoadedActivities.Common.Models;
using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.LoadedActivities.Persistence;

public class LoadedActivitiesRepository(DataContext context) : ILoadedActivitiesRepository
{
    public async Task<LoadedActivity?> GetLastIwmsLoadedActivityAsync(CancellationToken cancellationToken)
    {
        return await context.LoadedActivities
            .Where(x => x.ActivitySource == ActivitySource.Iwms)
            .OrderBy(x => x.Created)
            .LastOrDefaultAsync(cancellationToken);
    }

    public async Task<LoadedActivity?> GetLastMyStockLoadedActivityAsync(CancellationToken cancellationToken)
    {
        return await context.LoadedActivities
            .Where(x => x.ActivitySource == ActivitySource.MyStock)
            .OrderBy(x => x.Created)
            .LastOrDefaultAsync(cancellationToken);
    }

    public async Task<LoadedActivity?> GetLastSagDynamicsLoadedActivityAsync(CancellationToken cancellationToken)
    {
        return await context.LoadedActivities
            .Where(x => x.ActivitySource == ActivitySource.Dynamics)
            .OrderBy(x => x.Created)
            .LastOrDefaultAsync(cancellationToken);
    }

    public async Task<LoadedActivity?> GetLoadedActivityByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.LoadedActivities
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetLoadedActivitiesAsync(LoadedActivitiesFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.LoadedActivities;

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}