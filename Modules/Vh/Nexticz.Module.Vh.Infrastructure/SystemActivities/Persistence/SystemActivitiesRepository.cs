using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.SystemActivities.Common.Models;
using Nexticz.Module.Vh.Contracts.SystemActivities;
using Nexticz.Module.Vh.Domain.SystemActivities;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.SystemActivities.Persistence;

public class SystemActivitiesRepository(DataContext context) : ISystemActivitiesRepository
{
    public async Task<SystemActivity?> GetSystemActivityByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.SystemActivities
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SystemActivityResponse?> GetSystemActivityResponseByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return await context.SystemActivities
            .Where(x => x.Id == id)
            .Select(x => new SystemActivityResponse
            {
                Id = x.Id,
                Name = x.Name,
                ActionCodeWms = x.ActionCodeWms,
                SystemType = x.SystemType,
                DepositorGroupId = x.DepositorGroupId,
                ActivityCategoryId = x.ActivityCategoryId,
                Type = x.Type,
                WhatToMeasure = x.WhatToMeasure,
                Unit = x.Unit,
                Coefficient = x.Coefficient,
                CutOff = x.CutOff
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetSystemActivitiesAsync(SystemActivitiesFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.SystemActivities
            .Select(x => new SystemActivityResponse
            {
                Id = x.Id,
                Name = x.Name,
                ActionCodeWms = x.ActionCodeWms,
                SystemType = x.SystemType,
                DepositorGroupId = x.DepositorGroupId,
                ActivityCategoryId = x.ActivityCategoryId,
                Type = x.Type,
                WhatToMeasure = x.WhatToMeasure,
                Unit = x.Unit,
                Coefficient = x.Coefficient,
                CutOff = x.CutOff
            });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}