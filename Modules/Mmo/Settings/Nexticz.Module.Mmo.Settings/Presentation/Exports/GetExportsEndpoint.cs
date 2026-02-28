using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Exports;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Exports.Queries.GetExports;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.Exports;

internal static class GetExportsEndpoint
{
    public static IEndpointRouteBuilder MapGetExports(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.ExportEndpoints.GetExports,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Export>(filteringParams, cancellationToken));
                    }
                    
                    var query = new GetExportsQuery(filteringParams);
                    var result = await sender.Send(query, cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(ExportResponseFactory.Create));
                })
            .Produces<FilteredResult<ExportResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ExportEndpoints.GetExports)));

        return builder;
    }
}