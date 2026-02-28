using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Washing.Application.Batches;
using Nexticz.Module.Mmo.Washing.Application.Batches.Commands.FinishKit;


namespace Nexticz.Module.Mmo.Washing.Presentation.Batches;

internal static class FinishKitEndpoint
{
    public static IEndpointRouteBuilder MapFinishKitEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(WashingEndpoints.BatchesEndpoints.FinishKit,
                async (
                    Guid batchId,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var resultSisterBatch = await sender.Send(
                        new FinishKitCommand(batchId), 
                        cancellationToken);
                    
                    return resultSisterBatch.Match(
                        kitWashCycle => Results.Ok(new FinishKitResponse(KitWashCycleContractFactory.Create(kitWashCycle))),
                        ResultsHelper.Problem);
                })
            .Produces<FinishKitResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(WashingEndpoints.GetOpenApiName(nameof(WashingEndpoints.BatchesEndpoints.FinishKit)));

        return builder;
    }
}