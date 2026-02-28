using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Reporting.Contracts.Shifts;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftById;


namespace Nexticz.Module.Mmo.Reporting.Presentation.Shifts;

internal static class GetShiftDetailEndpoint
{
    public static IEndpointRouteBuilder MapGetShiftDetailEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ReportingEndpoints.ShiftEndpoints.GetShiftDetail,
                async (
                    Guid shiftId,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var shift = 
                        await sender.Send(
                            new GetShiftByIdQuery(shiftId), cancellationToken);

                    if (shift.IsError)
                        return ResultsHelper.Problem(shift.Errors);
                    
                    return Results.Ok(await ShiftDetailResponseFactory.CreateAsync(shift.Value, sender, cancellationToken));
                })
            .Produces<ShiftDetailResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(ReportingEndpoints.GetOpenApiName(nameof(ReportingEndpoints.ShiftEndpoints.GetShiftDetail)));

        return builder;
    }
}