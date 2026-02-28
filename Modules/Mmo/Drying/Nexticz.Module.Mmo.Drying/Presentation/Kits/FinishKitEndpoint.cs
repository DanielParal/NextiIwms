using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Drying.Application.Kits.Commands.FinishKit;

namespace Nexticz.Module.Mmo.Drying.Presentation.Kits;

internal static class FinishKitEndpoint
{
    public static IEndpointRouteBuilder MapFinishKitEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(DryingEndpoints.KitEndpoints.FinishKit,
                async (
                    [FromRoute] Guid kitId,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await sender.Send(new FinishKitCommand(kitId), cancellationToken);
                    
                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(DryingEndpoints.GetOpenApiName(nameof(DryingEndpoints.KitEndpoints.FinishKit)));

        return builder;
    }
}