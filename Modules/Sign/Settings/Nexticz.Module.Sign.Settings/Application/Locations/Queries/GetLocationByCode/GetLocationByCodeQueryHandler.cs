using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Locations.Queries.GetLocationByCode;

internal class GetLocationByCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetLocationByCodeQuery, ErrorOr<Location>>
{
    public async Task<ErrorOr<Location>> Handle(GetLocationByCodeQuery request, CancellationToken cancellationToken)
    {
        var location = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<Location>(
                x => x.Code.Equals(request.Code, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);

        if (location is null)
            return LocationErrors.CodeDoesNotExist;
        
        return location;
    }
}