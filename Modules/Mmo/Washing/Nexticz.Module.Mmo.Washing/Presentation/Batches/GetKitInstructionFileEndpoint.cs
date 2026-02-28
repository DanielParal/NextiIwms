using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchById;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetKitInstructionFile;


namespace Nexticz.Module.Mmo.Washing.Presentation.Batches;

internal static class GetKitInstructionFileEndpoint
{
    public static IEndpointRouteBuilder MapGetKitInstructionFileEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(WashingEndpoints.BatchesEndpoints.GetKitInstructionFile,
                async (
                    Guid batchId,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var batch = await sender.Send(new GetBatchByIdQuery(batchId), cancellationToken);
                    if (batch.IsError)
                        return ResultsHelper.Problem(batch.Errors);
                    
                    var kitInstructionResult = 
                        await sender.Send(new GetKitInstructionFileQuery(batch.Value.KitCode), cancellationToken);
                    
                    return kitInstructionResult.Match(
                        instructionFile => 
                            Results.Ok(new FileResponse(instructionFile.ContentBytes, instructionFile.ContentType, instructionFile.FileName)),
                        ResultsHelper.Problem);
                })
            .Produces<FileResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(WashingEndpoints.GetOpenApiName(nameof(WashingEndpoints.BatchesEndpoints.GetKitInstructionFile)));

        return builder;
    }
}