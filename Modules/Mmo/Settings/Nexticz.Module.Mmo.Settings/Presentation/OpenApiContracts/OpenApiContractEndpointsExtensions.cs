using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.OpenApiContracts;

internal static class OpenApiContractEndpointsExtensions
{
    public static IEndpointRouteBuilder MapOpenApiContractEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetOpenApiContractsEndpoint();
    }
}