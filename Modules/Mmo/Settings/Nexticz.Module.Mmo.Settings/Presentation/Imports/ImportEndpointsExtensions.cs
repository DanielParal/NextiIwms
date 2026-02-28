using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.Imports;

internal static class ImportEndpointsExtensions
{
    public static IEndpointRouteBuilder MapImportsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateImport()
            .MapGetImports();
    }
}