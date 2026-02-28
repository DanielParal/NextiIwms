using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.NonDispensingActivities;

public static class NonDispensingActivitiesExtensions
{
    public static IEndpointRouteBuilder MapNonDispensingActivitiesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateNonDispensingActivity()
            .MapUpdateNonDispensingActivity()
            .MapDeleteNonDispensingActivity()
            .MapGetNonDispensingActivities()
            .MapGetNonDispensingActivityByIdOrSlug();
    }
}