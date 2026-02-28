using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkers;

internal class GetWorkersQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IRequestHandler<GetWorkersQuery, FilteredResult<Worker>>
{
    public async Task<FilteredResult<Worker>> Handle(GetWorkersQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<Worker>(request.FilteringParams, cancellationToken);
    }
}