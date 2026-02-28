using Nexticz.Module.Vh.Contracts.ActivityCategories;
using Nexticz.Module.Vh.Domain.ActivityCategories;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.ActivityCategories.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IActivityCategoriesRepository
{
    Task<ActivityCategory?> GetActivityCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ActivityCategoryResponse?> GetActivityCategoryResponseByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<FilteredResult> GetActivityCategoriesAsync(ActivityCategoriesFilteringParams filteringParams,
        CancellationToken cancellationToken);
}