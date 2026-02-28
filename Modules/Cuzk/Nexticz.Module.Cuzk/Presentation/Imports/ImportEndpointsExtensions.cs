using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Cuzk.Presentation.Imports;

internal static class ImportEndpointsExtensions
{
    public static IEndpointRouteBuilder MapImportsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapRequestImport()
            .MapGetImportsEndpoint();
    }
}