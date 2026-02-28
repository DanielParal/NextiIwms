using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.Exports;

internal static class ExportEndpointsExtensions
{
    public static IEndpointRouteBuilder MapExportsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateExport()
            .MapGetExports();
    }
}