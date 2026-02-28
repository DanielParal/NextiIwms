using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Reporting.Contracts.Shifts;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Reporting.Application.Grouping;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftSummaries;


namespace Nexticz.Module.Mmo.Reporting.Presentation.Shifts;

internal static class GetShiftSummariesEndpoint
{
    public static IEndpointRouteBuilder MapGetShiftSummariesEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ReportingEndpoints.ShiftEndpoints.GetShiftsSummaries,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] IReportingGroupedResultHandler groupedResultHandler) =>
                {
                    // if (filteringParams.Group is not null)
                    // {
                    //     return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Shift>(filteringParams, cancellationToken));
                    // }
                    
                    var result = await sender.Send(new GetShiftSummariesQuery(filteringParams), cancellationToken);
                    return Results.Ok(result.MapDataFromTInToTOut(ShiftSummaryResponseFactory.Create));
                })
            .Produces<FilteredResult<ShiftSummaryResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(ReportingEndpoints.GetOpenApiName(nameof(ReportingEndpoints.ShiftEndpoints.GetShiftsSummaries)));

        return builder;
    }
}