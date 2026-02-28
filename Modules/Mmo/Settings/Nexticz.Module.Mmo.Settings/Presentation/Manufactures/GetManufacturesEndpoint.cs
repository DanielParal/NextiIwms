using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Manufactures;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactures;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.Manufactures;

internal static class GetManufacturesEndpoint
{
    public static IEndpointRouteBuilder MapGetManufactures(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.ManufactureEndpoints.GetManufactures,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler
                ) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Manufacture>(filteringParams, cancellationToken));
                    }
                    
                    var request = new GetManufacturesQuery(filteringParams);
                    var result = await mediator.Send(request, cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(ManufactureResponseFactory.Create));
                })
            .Produces<FilteredResult<ManufactureResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ManufactureEndpoints.GetManufactures)));

        return builder;
    }
}