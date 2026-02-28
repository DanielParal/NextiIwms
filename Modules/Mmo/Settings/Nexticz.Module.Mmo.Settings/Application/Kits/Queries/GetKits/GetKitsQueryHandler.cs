using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;


namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKits;

internal class GetKitsQueryHandler (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IRequestHandler<GetKitsQuery, FilteredResult<Kit>>
{
    public async Task<FilteredResult<Kit>> Handle(GetKitsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<Kit>(request.FilteringParams, cancellationToken); 
    }
}