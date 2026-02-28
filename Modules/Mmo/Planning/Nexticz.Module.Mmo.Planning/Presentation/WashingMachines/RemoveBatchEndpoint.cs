using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.RemoveBatch;


namespace Nexticz.Module.Mmo.Planning.Presentation.WashingMachines;

internal static class RemoveBatchEndpoint
{
    public static IEndpointRouteBuilder MapRemoveBatchEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(PlanningEndpoints.WashingMachineEndpoints.RemoveBatch,
                async (
                    [FromRoute] string washingMachineCode,
                    [FromRoute] Guid batchId,
                    [FromBody] RemoveBatchRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await sender.Send(new RemoveBatchCommand(request.LineQueueCode, batchId), cancellationToken);
                    
                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(PlanningEndpoints.GetOpenApiName(nameof(PlanningEndpoints.WashingMachineEndpoints.RemoveBatch)));

        return builder;
    }
}