using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.ApproveShift;

namespace Nexticz.Module.Mmo.Reporting.Presentation.Shifts;

internal static class ApproveShiftEndpoint
{
    public static IEndpointRouteBuilder MapApproveShiftEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ReportingEndpoints.ShiftEndpoints.ApproveShift,
                async (
                    Guid shiftId,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var approveResult = await sender.Send(new ApproveShiftCommand(shiftId), cancellationToken);

                    return approveResult.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(ReportingEndpoints.GetOpenApiName(nameof(ReportingEndpoints.ShiftEndpoints.ApproveShift)));

        return builder;
    }
}