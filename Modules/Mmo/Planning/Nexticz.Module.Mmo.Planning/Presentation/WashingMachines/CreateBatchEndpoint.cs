using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches.CreateSingleBatch;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches.CreateSisterBatches;
using Nexticz.Module.Mmo.Planning.Presentation.WashingMachines.ResponseFactories;


namespace Nexticz.Module.Mmo.Planning.Presentation.WashingMachines;

internal static class CreateBatchEndpoint
{
    public static IEndpointRouteBuilder MapCreateBatchEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(PlanningEndpoints.WashingMachineEndpoints.CreateBatch,
                async (
                    [FromRoute] string washingMachineCode,
                    [FromBody] CreateBatchRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    if (string.IsNullOrWhiteSpace(request.SisterPackagingCode))
                    {
                        var resultSingleBatch = await sender.Send(
                            new CreateSingleBatchCommand(
                                request.LineQueueCode, request.KitCode, request.KitsCount, request.PackagingCode), 
                            cancellationToken);
                        
                        return resultSingleBatch.Match(
                            batch => Results.Ok(BatchCreateResponseFactory.Create(batch)),
                            ResultsHelper.Problem);
                    }
                       
                    var resultSisterBatch = await sender.Send(
                        new CreateSisterBatchesCommand(
                            request.LineQueueCode, request.KitCode, request.KitsCount, request.PackagingCode, request.SisterPackagingCode), 
                        cancellationToken);
                    
                    return resultSisterBatch.Match(
                        sisterBatches => 
                            Results.Ok(
                                BatchCreateResponseFactory.Create(sisterBatches.Batch, sisterBatches.SisterBatch)),
                        ResultsHelper.Problem);
                })
            .Produces<CreateBatchResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(PlanningEndpoints.GetOpenApiName(nameof(PlanningEndpoints.WashingMachineEndpoints.CreateBatch)));

        return builder;
    }
}