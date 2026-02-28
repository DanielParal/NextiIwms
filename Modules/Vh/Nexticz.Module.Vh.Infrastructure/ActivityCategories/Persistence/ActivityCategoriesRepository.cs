using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.ActivityCategories.Common.Models;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Contracts.ActivityCategories;
using Nexticz.Module.Vh.Contracts.WorkerShifts;
using Nexticz.Module.Vh.Domain.ActivityCategories;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.ActivityCategories.Persistence;

public class ActivityCategoriesRepository(DataContext context) : IActivityCategoriesRepository
{
    public async Task<ActivityCategory?> GetActivityCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.ActivityCategories
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ActivityCategoryResponse?> GetActivityCategoryResponseByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return await context.ActivityCategories
            .Where(x => x.Id == id)
            .Select(x => new ActivityCategoryResponse
            {
                Id = x.Id, Name = x.Name, Color = x.Color,
                ActivityType = Enum.Parse<ActivityType>(x.ActivityType.ToString())
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetActivityCategoriesAsync(ActivityCategoriesFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.ActivityCategories
            .Select(x => new ActivityCategoryResponse
            {
                Id = x.Id, Name = x.Name, Color = x.Color,
                ActivityType = Enum.Parse<ActivityType>(x.ActivityType.ToString())
            });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}