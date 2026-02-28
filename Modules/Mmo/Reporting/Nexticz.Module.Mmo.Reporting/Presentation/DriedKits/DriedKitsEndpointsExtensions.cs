using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Reporting.Presentation.DriedKits;

internal static class DriedKitsEndpointsExtensions
{
    public static IEndpointRouteBuilder MapDriedKitsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetDriedKitsEndpoint();
    }
}