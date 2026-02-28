using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.FinishBatch;


namespace Nexticz.Module.Mmo.Planning.Presentation.WashingMachines;

internal static class FinishBatchEndpoint
{
    public static IEndpointRouteBuilder MapFinishBatchEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(PlanningEndpoints.WashingMachineEndpoints.FinishBatch,
                async (
                    [FromRoute] string washingMachineCode,
                    [FromRoute] Guid batchId,
                    [FromBody] FinishBatchRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await sender.Send(new FinishBatchCommand(batchId, request.LineQueueCode), cancellationToken);
                    
                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(PlanningEndpoints.GetOpenApiName(nameof(PlanningEndpoints.WashingMachineEndpoints.FinishBatch)));

        return builder;
    }
}