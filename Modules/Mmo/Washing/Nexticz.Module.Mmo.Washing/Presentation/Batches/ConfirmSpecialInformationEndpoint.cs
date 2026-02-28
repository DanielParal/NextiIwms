using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Washing.Application.Batches.Commands.ConfirmSpecialInformation;


namespace Nexticz.Module.Mmo.Washing.Presentation.Batches;

internal static class ConfirmSpecialInformationEndpoint
{
    public static IEndpointRouteBuilder MapConfirmSpecialInformationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(WashingEndpoints.BatchesEndpoints.ConfirmSpecialInformation,
                async (
                    Guid batchId,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new ConfirmSpecialInformationCommand(batchId), cancellationToken);
                    
                    return result.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(WashingEndpoints.GetOpenApiName(nameof(WashingEndpoints.BatchesEndpoints.ConfirmSpecialInformation)));

        return builder;
    }
}