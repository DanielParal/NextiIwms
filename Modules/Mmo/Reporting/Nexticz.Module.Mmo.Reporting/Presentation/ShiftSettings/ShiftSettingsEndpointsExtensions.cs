using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Reporting.Presentation.DriedKits;

namespace Nexticz.Module.Mmo.Reporting.Presentation.ShiftSettings;

internal static class ShiftSettingsEndpointsExtensions
{
    public static IEndpointRouteBuilder MapShiftSettingsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetShiftSettingsEndpoint();
    }
}