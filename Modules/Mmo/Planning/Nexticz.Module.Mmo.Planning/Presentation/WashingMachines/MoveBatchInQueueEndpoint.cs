using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.MoveBatchInQueue;


namespace Nexticz.Module.Mmo.Planning.Presentation.WashingMachines;

internal static class MoveBatchInQueueEndpoint
{
    public static IEndpointRouteBuilder MapMoveBatchInQueueEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(PlanningEndpoints.WashingMachineEndpoints.MoveBatchInQueue,
                async (
                    [FromRoute] string washingMachineCode,
                    [FromRoute] Guid batchId,
                    [FromBody] MoveBatchInQueueRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var command = new MoveBatchInQueueCommand(request.LineQueueCode, batchId, request.Movement);
                    var result = await sender.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(PlanningEndpoints.GetOpenApiName(nameof(PlanningEndpoints.WashingMachineEndpoints.MoveBatchInQueue)));

        return builder;
    }
}