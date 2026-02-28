using Nexticz.Module.Vh.Contracts.Workers;
using Nexticz.Module.Vh.Domain.Workers;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Workers.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IWorkersRepository
{
    Task<Worker?> GetWorkerByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<WorkerResponse?> GetWorkerResponseByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<WorkerResponse?> GetWorkerResponseBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<FilteredResult> GetWorkersAsync(WorkersFilteringParams filteringParams,
        CancellationToken cancellationToken);
}