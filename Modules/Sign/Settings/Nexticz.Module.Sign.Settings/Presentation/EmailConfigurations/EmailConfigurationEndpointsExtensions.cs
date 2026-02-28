using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailConfigurations;

internal static class EmailConfigurationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapEmailConfigurationsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetEmailConfigurationsEndpoint()
            .MapCreateEmailConfigurationEndpoint()
            .MapUpdateEmailConfigurationEndpoint()
            .MapDeleteEmailConfigurationEndpoint()
            .MapGetEmailConfigurationByIdEndpoint();
    }
}