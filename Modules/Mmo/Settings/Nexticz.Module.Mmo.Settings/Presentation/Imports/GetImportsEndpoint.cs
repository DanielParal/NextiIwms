using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Imports;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.Queries.GetImports;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.Imports;

internal static class GetImportsEndpoint
{
    public static IEndpointRouteBuilder MapGetImports(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.ImportEndpoints.GetImports,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Import>(filteringParams, cancellationToken));
                    }
                    
                    var query = new GetImportsQuery(filteringParams);
                    var result = await mediator.Send(query, cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(ImportResponseFactory.Create));
                })
            .Produces<FilteredResult<ImportResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ImportEndpoints.GetImports)));

        return builder;
    }
}