using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Drying.Application.Kits.Commands.TransferKitToDryingSection;

namespace Nexticz.Module.Mmo.Drying.Presentation.Kits;

internal static class TransferKitEndpoint
{
    public static IEndpointRouteBuilder MapTransferKitEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(DryingEndpoints.KitEndpoints.TransferKit,
                async (
                    [FromRoute] Guid kitId,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await sender.Send(new TransferKitToDryingSectionCommand(kitId), cancellationToken);
                    
                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(DryingEndpoints.GetOpenApiName(nameof(DryingEndpoints.KitEndpoints.TransferKit)));

        return builder;
    }
}