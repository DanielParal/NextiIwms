using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.Municipalities;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Cuzk.Application.Grouping;
using Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalities;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Presentation.Municipalities;

internal static class GetMunicipalitiesEndpoint
{
    public static IEndpointRouteBuilder MapGetMunicipalitiesEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(CuzkEndpoints.MunicipalityEndpoints.GetMunicipalities,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ICuzkGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Municipality>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetMunicipalitiesQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(MunicipalityResponseFactory.Create));
                })
            .Produces<FilteredResult<MunicipalityResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.MunicipalityEndpoints.GetMunicipalities)));

        return builder;
    }
}