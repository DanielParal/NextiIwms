using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.KitTypes;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.KitTypes.Queries.GetKitTypes;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.KitTypes;

internal static class GetKitTypesEndpoint
{
    public static IEndpointRouteBuilder MapGetKitTypes(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.KitTypeEndpoints.GetKitTypes,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<KitType>(filteringParams, cancellationToken));
                    }
                    
                    var query = new GetKitTypesQuery(filteringParams);
                    var result = await mediator.Send(query, cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(KitTypeResponseFactory.Create));
                })
            .Produces<FilteredResult<KitTypeResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitTypeEndpoints.GetKitTypes)));

        return builder;
    }
}