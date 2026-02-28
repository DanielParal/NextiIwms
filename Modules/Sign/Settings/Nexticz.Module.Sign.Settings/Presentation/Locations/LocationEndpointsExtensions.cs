using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.Locations;

internal static class LocationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapLocationsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateLocationEndpoint()
            .MapGetLocationByCodeEndpoint()
            .MapGetLocationsEndpoint()
            .MapUpdateLocationEndpoint()
            .MapDeleteLocationEndpoint();
    }
}