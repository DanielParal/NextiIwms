using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.LoadedActivities.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface ILoadedActivitiesRepository
{
    Task<LoadedActivity?> GetLastIwmsLoadedActivityAsync(CancellationToken cancellationToken);
    Task<LoadedActivity?> GetLastMyStockLoadedActivityAsync(CancellationToken cancellationToken);
    Task<LoadedActivity?> GetLastSagDynamicsLoadedActivityAsync(CancellationToken cancellationToken);
    Task<LoadedActivity?> GetLoadedActivityByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<FilteredResult> GetLoadedActivitiesAsync(LoadedActivitiesFilteringParams filteringParams,
        CancellationToken cancellationToken);
}