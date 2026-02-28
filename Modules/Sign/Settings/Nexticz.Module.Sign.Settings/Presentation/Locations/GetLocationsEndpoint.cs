using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Locations;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Sign.Settings.Application.Grouping;
using Nexticz.Module.Sign.Settings.Application.Locations.Queries.GetLocations;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;


namespace Nexticz.Module.Sign.Settings.Presentation.Locations;

internal static class GetLocationsEndpoint
{
    public static IEndpointRouteBuilder MapGetLocationsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.LocationEndpoints.GetLocations,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Location>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetLocationsQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(LocationResponseFactory.Create));
                })
            .Produces<FilteredResult<LocationResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.LocationEndpoints.GetLocations)));

        return builder;
    }
}