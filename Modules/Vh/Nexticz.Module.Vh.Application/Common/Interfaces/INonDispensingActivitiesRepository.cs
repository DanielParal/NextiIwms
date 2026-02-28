using Nexticz.Module.Vh.Contracts.NonDispensingActivities;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface INonDispensingActivitiesRepository
{
    Task<NonDispensingActivity?> GetNonDispensingActivityByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<NonDispensingActivityResponse?> GetNonDispensingActivityResponseByIdAsync(Guid id,
        CancellationToken cancellationToken);

    Task<NonDispensingActivityResponse?> GetNonDispensingActivityResponseBySlugAsync(string slug,
        CancellationToken cancellationToken);

    Task<FilteredResult> GetNonDispensingActivitiesAsync(NonDispensingActivitiesFilteringParams filteringParams,
        CancellationToken cancellationToken);

    Task<FilteredResult> GetNonDispensingActivitiesResponseAsync(NonDispensingActivitiesFilteringParams filteringParams,
        CancellationToken cancellationToken);
}