using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Drying.Contracts.Kits;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Drying.Application.Kits.Queries.GetKitByCompletedKitsCount;

namespace Nexticz.Module.Mmo.Drying.Presentation.Kits;

internal static class GetKitByCompletedKitsCountEndpoint
{
    public static IEndpointRouteBuilder MapGetKitByCompletedKitsCountEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(DryingEndpoints.KitEndpoints.GetKitByCompletedKitsCount,
                async (
                    int completedKitsCount,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] IClock clock
                ) =>
                {
                    var result = await sender.Send(new GetKitByCompletedKitsCountQuery(completedKitsCount), cancellationToken);
                    
                    return result.Match(
                        kit => Results.Ok(KitResponseFactory.Create(kit, clock.TenantNowOffset)),
                        ResultsHelper.Problem);
                })
            .Produces<KitResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(DryingEndpoints.GetOpenApiName(nameof(DryingEndpoints.KitEndpoints.GetKitByCompletedKitsCount)));

        return builder;
    }
}