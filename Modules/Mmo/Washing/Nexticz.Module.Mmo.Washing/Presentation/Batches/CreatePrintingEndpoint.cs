using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Washing.Application.Batches.Commands.CreatePrinting;


namespace Nexticz.Module.Mmo.Washing.Presentation.Batches;

public static class CreatePrintingEndpoint
{
    public static IEndpointRouteBuilder MapCreatePrintingEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(WashingEndpoints.BatchesEndpoints.CreatePrinting,
                async (
                    Guid batchId,
                    CreatePrintingRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreatePrintingCommand(request.LineCode, request.KitWashCycleId), cancellationToken);
                    
                    return result.Match(
                        printingCommandResponse => Results.Ok(CreatePrintingResponseFactory.Create(printingCommandResponse)),
                        ResultsHelper.Problem);
                })
            .Produces<CreatePrintingResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(WashingEndpoints.GetOpenApiName(nameof(WashingEndpoints.BatchesEndpoints.CreatePrinting)));

        return builder;
    }
}