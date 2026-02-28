using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocations;

internal class GetAddressLocationsQueryHandler(ICuzkReadOnlyEventStoreRepository readOnlyRepository)
    : IRequestHandler<GetAddressLocationsQuery, FilteredResult<AddressLocation>>
{
    public async Task<FilteredResult<AddressLocation>> Handle(GetAddressLocationsQuery query, CancellationToken cancellationToken)
    {
        return await readOnlyRepository.GetFilteredAsync<AddressLocation>(query.FilteringParams, cancellationToken);
    }
}