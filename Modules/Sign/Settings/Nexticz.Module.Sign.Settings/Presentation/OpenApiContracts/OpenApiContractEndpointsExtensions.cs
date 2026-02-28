using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.OpenApiContracts;

internal static class OpenApiContractEndpointsExtensions
{
    public static IEndpointRouteBuilder MapOpenApiContractEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetOpenApiContractsEndpoint();
    }
}