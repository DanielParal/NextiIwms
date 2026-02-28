using Nexticz.Module.Sign.Settings.Contracts.Locations;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.Locations;

internal static class LocationResponseFactory
{
    public static LocationResponse Create(Location location)
    {
        return new LocationResponse(location.Id, location.Code, location.Name);
    }
}