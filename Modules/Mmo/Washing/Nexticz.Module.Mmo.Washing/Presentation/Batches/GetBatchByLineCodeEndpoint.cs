using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchResponseByLineCode;

namespace Nexticz.Module.Mmo.Washing.Presentation.Batches;

internal static class GetBatchByLineCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetBatchByLineCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(WashingEndpoints.BatchesEndpoints.GetBatchByLineCode,
                async (
                    [AsParameters] GetBatchByLineCodeFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var resultBatch = 
                        await sender.Send(
                            new GetBatchResponseByLineCodeQuery(filteringParams.LineCode), cancellationToken);
                    
                    if (resultBatch == null)
                        return Results.Ok();

                    return Results.Ok(resultBatch);
                })
            .Produces<BatchResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(WashingEndpoints.GetOpenApiName(nameof(WashingEndpoints.BatchesEndpoints.GetBatchByLineCode)));

        return builder;
    }
}