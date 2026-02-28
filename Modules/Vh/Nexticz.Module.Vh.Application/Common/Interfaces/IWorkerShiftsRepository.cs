using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.WorkerShifts.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IWorkerShiftsRepository
{
    Task<WorkerShift?> GetWorkerShiftByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<FilteredResult> GetWorkerShiftsAsync(WorkerShiftsFilteringParams filteringParams,
        CancellationToken cancellationToken);

    Task<List<WorkerShift>> GetWorkerShiftsWithAnyNotEndedActivityAsync(CancellationToken cancellationToken = default);

    Task<WorkerShiftActivity?>
        GetWorkerShiftActivityByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken);
    Task<WorkerShift?> GetWorkerShiftByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken);
}