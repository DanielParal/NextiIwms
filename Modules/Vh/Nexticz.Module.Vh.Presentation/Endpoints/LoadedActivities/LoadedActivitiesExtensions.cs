using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadedActivities;

public static class LoadedActivitiesExtensions
{
    public static IEndpointRouteBuilder MapLoadedActivitiesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetLoadedActivities()
            .MapUpdateLoadedActivityEndpoint()
            .MapImportLoadedActivitiesEndpoint();
    }
}