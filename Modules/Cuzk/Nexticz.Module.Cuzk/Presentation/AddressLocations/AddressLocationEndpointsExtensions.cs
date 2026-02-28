using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Cuzk.Presentation.AddressLocations;

internal static class AddressLocationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapAddressLocationsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateAddressLocationEndpoint()
            .MapUpdateAddressLocationEndpoint()
            .MapDeleteAddressLocationEndpoint()
            .MapGetAddressLocationByCodeEndpoint()
            .MapGetAddressLocationsEndpoint();
    }
}