using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Common.Models;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.NonDispensingActivities.Persistence;

public class NonDispensingActivitiesRepository(DataContext context) : INonDispensingActivitiesRepository
{
    public async Task<NonDispensingActivity?> GetNonDispensingActivityByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return await context.NonDispensingActivities
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<NonDispensingActivityResponse?> GetNonDispensingActivityResponseByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return await context.NonDispensingActivities
            .Where(x => x.Id == id)
            .Select(x => new NonDispensingActivityResponse
            {
                Id = x.Id,
                ActivityIdentifier = x.ActivityIdentifier,
                Name = x.Name,
                RequireNote = x.RequireNote,
                Note = x.Note,
                Unit = x.Unit,
                Coefficient = x.Coefficient,
                CutOff = x.CutOff,
                CenterId = x.CenterId,
                ActivityCategoryId = x.ActivityCategoryId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<NonDispensingActivityResponse?> GetNonDispensingActivityResponseBySlugAsync(string slug,
        CancellationToken cancellationToken)
    {
        return await context.NonDispensingActivities
            .Where(x => x.ActivityIdentifier == slug)
            .Select(x => new NonDispensingActivityResponse
            {
                Id = x.Id,
                ActivityIdentifier = x.ActivityIdentifier,
                Name = x.Name,
                RequireNote = x.RequireNote,
                Note = x.Note,
                Unit = x.Unit,
                Coefficient = x.Coefficient,
                CutOff = x.CutOff,
                CenterId = x.CenterId,
                ActivityCategoryId = x.ActivityCategoryId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetNonDispensingActivitiesAsync(
        NonDispensingActivitiesFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.NonDispensingActivities
            .Include(x => x.Center)
            .Include(x => x.ActivityCategory);

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }

    public async Task<FilteredResult> GetNonDispensingActivitiesResponseAsync(
        NonDispensingActivitiesFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.NonDispensingActivities
            .Select(x => new NonDispensingActivityResponse
            {
                Id = x.Id,
                ActivityIdentifier = x.ActivityIdentifier,
                Name = x.Name,
                RequireNote = x.RequireNote,
                Note = x.Note,
                Unit = x.Unit,
                Coefficient = x.Coefficient,
                CutOff = x.CutOff,
                CenterId = x.CenterId,
                ActivityCategoryId = x.ActivityCategoryId
            });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}