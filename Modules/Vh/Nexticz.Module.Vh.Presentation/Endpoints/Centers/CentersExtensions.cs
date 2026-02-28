using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Centers;

public static class CentersExtensions
{
    public static IEndpointRouteBuilder MapCentersEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateCenter()
            .MapUpdateCenter()
            .MapDeleteCenter()
            .MapGetCenterById()
            .MapGetCenters();
    }
}