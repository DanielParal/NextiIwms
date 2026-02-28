using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;


namespace Nexticz.Module.Sign.Settings.Application.Locations.Queries.GetLocations;

internal record GetLocationsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Location>>;