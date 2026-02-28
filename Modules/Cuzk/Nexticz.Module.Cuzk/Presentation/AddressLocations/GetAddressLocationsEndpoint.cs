using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.AddressLocations;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocations;
using Nexticz.Module.Cuzk.Application.Grouping;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Presentation.AddressLocations;

internal static class GetAddressLocationsEndpoint
{
    public static IEndpointRouteBuilder MapGetAddressLocationsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(CuzkEndpoints.AddressLocationEndpoints.GetAddressLocations,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ICuzkGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<AddressLocation>(filteringParams, cancellationToken));
                    }

                    var result = await sender.Send(new GetAddressLocationsQuery(filteringParams), cancellationToken);

                    return Results.Ok(result.MapDataFromTInToTOut(AddressLocationResponseFactory.Create));
                })
            .Produces<FilteredResult<AddressLocationResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.AddressLocationEndpoints.GetAddressLocations)));

        return builder;
    }
}