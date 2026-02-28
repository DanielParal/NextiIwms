using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Cuzk.Presentation.AddressLocationSlugs;

internal static class AddressLocationSlugEndpointsExtensions
{
    public static IEndpointRouteBuilder MapAddressLocationSlugsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetAddressLocationSlugsEndpoint();
    }
}