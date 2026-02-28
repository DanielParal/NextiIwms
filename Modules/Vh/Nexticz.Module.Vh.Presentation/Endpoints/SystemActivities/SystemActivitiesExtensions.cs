using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.SystemActivities;

public static class SystemActivitiesExtensions
{
    public static IEndpointRouteBuilder MapSystemActivitiesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateSystemActivity()
            .MapUpdateSystemActivity()
            .MapDeleteSystemActivity()
            .MapGetSystemActivities()
            .MapGetSystemActivityById();
    }
}