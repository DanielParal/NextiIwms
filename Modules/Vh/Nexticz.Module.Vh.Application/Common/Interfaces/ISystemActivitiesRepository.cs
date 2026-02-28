using Nexticz.Module.Vh.Contracts.SystemActivities;
using Nexticz.Module.Vh.Domain.SystemActivities;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.SystemActivities.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface ISystemActivitiesRepository
{
    Task<SystemActivity?> GetSystemActivityByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<SystemActivityResponse?> GetSystemActivityResponseByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<FilteredResult> GetSystemActivitiesAsync(SystemActivitiesFilteringParams filteringParams,
        CancellationToken cancellationToken);
}