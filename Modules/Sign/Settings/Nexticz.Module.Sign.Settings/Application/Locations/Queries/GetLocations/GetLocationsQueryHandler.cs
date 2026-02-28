using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;


namespace Nexticz.Module.Sign.Settings.Application.Locations.Queries.GetLocations;

internal class GetLocationsQueryHandler
    (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetLocationsQuery, FilteredResult<Location>>
{
    public async Task<FilteredResult<Location>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<Location>(request.FilteringParams, cancellationToken);
    }
}