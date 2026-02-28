using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.BackgroundServices;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Washing.Application.Simulations;

namespace Nexticz.Module.Mmo.Washing.Presentation.Simulations;

internal static class StartSimulationEndpoint
{
    public static IEndpointRouteBuilder MapStartSimulationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(WashingEndpoints.SimulationEndpoints.StartSimulation,
                async (
                    [FromServices] OnDemandBackgroundServiceHost<SimulationOnDemandBackgroundService> simulationOnDemandBackgroundService,
                    [FromServices] SimulationValidator validator,
                    CancellationToken cancellationToken
                    ) =>
                {
                    var workerResult = await validator.VerifyWorkersExistAsync(cancellationToken);
                    if (workerResult.IsError)
                        return ResultsHelper.Problem(workerResult.Errors);
                    
                    simulationOnDemandBackgroundService.Start();
                    return Results.Ok();
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(WashingEndpoints.GetOpenApiName(nameof(WashingEndpoints.SimulationEndpoints.StartSimulation)));

        return builder;
    }
}