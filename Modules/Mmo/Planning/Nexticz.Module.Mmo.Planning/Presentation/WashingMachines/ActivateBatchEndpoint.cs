using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.ActivateBatch;


namespace Nexticz.Module.Mmo.Planning.Presentation.WashingMachines;

internal static class ActivateBatchEndpoint
{
    public static IEndpointRouteBuilder MapActivateBatchEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(PlanningEndpoints.WashingMachineEndpoints.ActivateBatch,
                async (
                    [FromRoute] string washingMachineCode,
                    [FromRoute] Guid batchId,
                    [FromBody] ActivateBatchRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var resultActivateBatch = 
                        await sender.Send(new ActivateBatchCommand(batchId, request.LineQueueCode), cancellationToken);
                         
                    return resultActivateBatch.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<ActivateBatchResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(PlanningEndpoints.GetOpenApiName(nameof(PlanningEndpoints.WashingMachineEndpoints.ActivateBatch)));

        return builder;
    }
}