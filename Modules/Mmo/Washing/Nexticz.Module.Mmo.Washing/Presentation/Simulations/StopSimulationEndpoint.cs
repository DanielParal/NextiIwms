using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.BackgroundServices;
using Nexticz.Module.Mmo.Washing.Application.Simulations;

namespace Nexticz.Module.Mmo.Washing.Presentation.Simulations;

internal static class StopSimulationEndpoint
{
    public static IEndpointRouteBuilder MapStopSimulationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(WashingEndpoints.SimulationEndpoints.StopSimulation,
                ([FromServices] OnDemandBackgroundServiceHost<SimulationOnDemandBackgroundService> simulationOnDemandBackgroundService) =>
                {
                    simulationOnDemandBackgroundService.Stop();
                    return Results.Ok();
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(WashingEndpoints.GetOpenApiName(nameof(WashingEndpoints.SimulationEndpoints.StopSimulation)));

        return builder;
    }
}