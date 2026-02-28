using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Reporting.Contracts.Shifts;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftSelectionsByDate;

namespace Nexticz.Module.Mmo.Reporting.Presentation.Shifts;

internal static class GetShiftSelectionsByDateEndpoint
{
    public static IEndpointRouteBuilder MapGetShiftSelectionsByDateEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ReportingEndpoints.ShiftEndpoints.GetShiftSelections,
                async (
                    [AsParameters] GetShiftSelectionParams getShiftSelectionParams,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new GetShiftSelectionsByDateQuery(getShiftSelectionParams.SelectedDate), cancellationToken);
                    return Results.Ok(result);
                })
            .Produces<ShiftSelectionResponse[]>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(ReportingEndpoints.GetOpenApiName(nameof(ReportingEndpoints.ShiftEndpoints.GetShiftSelections)));

        return builder;
    }
}