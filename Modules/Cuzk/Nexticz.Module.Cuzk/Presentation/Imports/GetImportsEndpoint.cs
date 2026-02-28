using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.Imports;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Cuzk.Application.Grouping;
using Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Queries.GetImports;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Presentation.Imports;

internal static class GetImportsEndpoint
{
    public static IEndpointRouteBuilder MapGetImportsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(CuzkEndpoints.ImportEndpoints.GetImports,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ICuzkGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Import>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetImportsQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(ImportResponseFactory.Create));
                })
            .Produces<FilteredResult<ImportResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.ImportEndpoints.GetImports)));

        return builder;
    }
}