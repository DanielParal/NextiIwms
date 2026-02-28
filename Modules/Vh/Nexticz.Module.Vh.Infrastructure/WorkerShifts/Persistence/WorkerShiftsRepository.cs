using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.WorkerShifts.Common.Models;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.WorkerShifts.Persistence;

public class WorkerShiftsRepository(DataContext context) : IWorkerShiftsRepository
{
    public async Task<WorkerShift?> GetWorkerShiftByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.WorkerShifts
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetWorkerShiftsAsync(WorkerShiftsFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.WorkerShifts
            .Where(x =>
                filteringParams.ActivitiesStart == null ||
                filteringParams.ActivitiesEnd == null ||
                x.Activities.Any(y =>
                    (y.Start > filteringParams.ActivitiesStart && y.Start < filteringParams.ActivitiesEnd) ||
                    (y.End > filteringParams.ActivitiesStart && y.End < filteringParams.ActivitiesEnd)
                )
            )
            .Where(x =>
                filteringParams.FromYear == null ||
                x.End!.Value.Year == filteringParams.FromYear)
            .Where(x =>
                filteringParams.FromMonth == null ||
                x.End!.Value.Month == filteringParams.FromMonth)
            .Where(x =>
                filteringParams.ActivityCenter == null ||
                x.Activities.Any(y =>
                    y.CenterCode == filteringParams.ActivityCenter) ||
                x.WorkerCenterCode == filteringParams.ActivityCenter
            );

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }

    public async Task<List<WorkerShift>> GetWorkerShiftsWithAnyNotEndedActivityAsync(
        CancellationToken cancellationToken)
    {
        return await context.WorkerShifts
            .Where(x => x.Activities.Any(y => x.End == null))
            .ToListAsync(cancellationToken);
    }

    public async Task<WorkerShiftActivity?> GetWorkerShiftActivityByIdAsNoTrackingAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return await context.WorkerShifts
            .AsNoTracking()
            .SelectMany(x => x.Activities)
            .FirstOrDefaultAsync(y => y.Id == id, cancellationToken);
    }

    public async Task<WorkerShift?> GetWorkerShiftByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.WorkerShifts
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}