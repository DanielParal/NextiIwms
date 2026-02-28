using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocations;

internal record GetAddressLocationsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<AddressLocation>>;