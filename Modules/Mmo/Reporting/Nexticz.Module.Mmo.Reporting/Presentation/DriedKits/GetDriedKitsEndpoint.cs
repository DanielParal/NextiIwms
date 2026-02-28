using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Reporting.Contracts.DriedKits;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Reporting.Application.DriedKits.Queries.GetDriedKits;
using Nexticz.Module.Mmo.Reporting.Application.Grouping;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;


namespace Nexticz.Module.Mmo.Reporting.Presentation.DriedKits;

internal static class GetDriedKitsEndpoint
{
    public static IEndpointRouteBuilder MapGetDriedKitsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ReportingEndpoints.DriedKitEndpoints.GetDriedKits,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] IReportingGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<DriedKit>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetDriedKitsQuery(filteringParams), cancellationToken);
                    return Results.Ok(result.MapDataFromTInToTOut(DriedKitResponseFactory.Create));
                })
            .Produces<FilteredResult<DriedKitResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(ReportingEndpoints.GetOpenApiName(nameof(ReportingEndpoints.DriedKitEndpoints.GetDriedKits)));

        return builder;
    }
}