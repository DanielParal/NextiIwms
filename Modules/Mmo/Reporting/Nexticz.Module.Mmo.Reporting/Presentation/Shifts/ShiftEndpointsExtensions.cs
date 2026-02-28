using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Reporting.Presentation.Shifts;

internal static class ShiftEndpointsExtensions
{
    public static IEndpointRouteBuilder MapShiftsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetShiftDetailEndpoint()
            .MapGetShiftSummariesEndpoint()
            .MapApproveShiftEndpoint()
            .MapGetShiftSelectionsByDateEndpoint();
    }
}