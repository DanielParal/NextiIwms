using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.SagDynamicsActivities;

public static class SagDynamicsActivitiesExtensions
{
    public static IEndpointRouteBuilder MapSagDynamicsActivitiesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapLoadSagDynamicsActivities();
    }
}