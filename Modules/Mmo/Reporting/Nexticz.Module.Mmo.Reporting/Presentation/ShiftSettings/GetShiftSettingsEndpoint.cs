using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Reporting.Contracts.ShiftSettings;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings.Queries.GetShiftSettings;


namespace Nexticz.Module.Mmo.Reporting.Presentation.ShiftSettings;

internal static class GetShiftSettingsEndpoint
{
    public static IEndpointRouteBuilder MapGetShiftSettingsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ReportingEndpoints.ShiftSettingEndpoints.GetShiftSettings,
                async (
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new GetShiftSettingsQuery(), cancellationToken);
                    
                    return result.Match(
                        shiftSettings => Results.Ok(shiftSettings.Select(ShiftSettingContractFactory.Create)),
                        ResultsHelper.Problem);
                })
            .Produces<ShiftSettingContract[]>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(ReportingEndpoints.GetOpenApiName(nameof(ReportingEndpoints.ShiftSettingEndpoints.GetShiftSettings)));

        return builder;
    }
}