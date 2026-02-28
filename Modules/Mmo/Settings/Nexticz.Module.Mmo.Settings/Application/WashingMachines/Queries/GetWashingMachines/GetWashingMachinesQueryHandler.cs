using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;


namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachines;

internal class GetWashingMachinesQueryHandler (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IRequestHandler<GetWashingMachinesQuery, FilteredResult<WashingMachine>>
{
    public async Task<FilteredResult<WashingMachine>> Handle(GetWashingMachinesQuery request, CancellationToken cancellationToken)
    {
        var result = await readOnlyEventStoreRepository.GetFilteredAsync<WashingMachine>(request.FilteringParams, cancellationToken);
        result.Data = result.Data.OrderBy(x => x.Code).ToList();
        return result;
    }
}