using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Drying.Contracts.Kits;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Drying.Application.Grouping;
using Nexticz.Module.Mmo.Drying.Application.Kits.Queries.GetKits;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Drying.Presentation.Kits;

internal static class GetKitsEndpoint
{
    public static IEndpointRouteBuilder MapGetKitsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(DryingEndpoints.KitEndpoints.GetKits,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] IDryingGroupedResultHandler groupedResultHandler,
                    [FromServices] IClock clock
                ) =>
                {
                    if (filteringParams.Group is not null)
                        return Results.Ok(
                            await groupedResultHandler.GetGroupedResultAsync<Kit>(filteringParams, cancellationToken));

                    var result = await sender.Send(new GetKitsQuery(filteringParams), cancellationToken);
                    var mappedResultAndSorted =
                        result.MapDataFromTInToTOut(x => KitResponseFactory.Create(x, clock.TenantNowOffset));
                    mappedResultAndSorted.Data = mappedResultAndSorted.Data.OrderBy(x => x.RemainingDryingTime)
                        .ThenBy(x => x.CompletedKitsCount).ToList();
                    return Results.Ok(mappedResultAndSorted);
                })
            .Produces<FilteredResult<KitResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(DryingEndpoints.GetOpenApiName(nameof(DryingEndpoints.KitEndpoints.GetKits)));

        return builder;
    }
}