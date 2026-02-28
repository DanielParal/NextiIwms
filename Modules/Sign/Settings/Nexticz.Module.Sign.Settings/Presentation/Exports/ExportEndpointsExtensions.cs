using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.Exports;

internal static class ExportEndpointsExtensions
{
    public static IEndpointRouteBuilder MapExportEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateExport();
    }
}