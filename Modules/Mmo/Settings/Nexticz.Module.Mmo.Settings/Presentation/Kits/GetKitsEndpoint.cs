using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.Kits;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKits;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Presentation.Kits;

internal static class GetKitsEndpoint
{
    public static IEndpointRouteBuilder MapGetKits(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.KitEndpoints.GetKits,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler,
                    [FromServices] KitResponseFactory kitResponseFactory) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Kit>(filteringParams, cancellationToken));
                    }
                    
                    var query = new GetKitsQuery(filteringParams);
                    var result = await mediator.Send(query, cancellationToken);
                    
                    var kitResponses = await kitResponseFactory.CreateAsync(result.Data.ToArray(), cancellationToken);
                    return Results.Ok(result.MapDataFromTInToTOut<Kit, KitResponse>(kitResponses));
                })
            .Produces<FilteredResult<KitResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitEndpoints.GetKits)));

        return builder;
    }
}