using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.Emails;

internal static class EmailEndpointsExtensions
{
    public static IEndpointRouteBuilder MapEmailsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetEmailsEndpoint()
            .MapSendEmailsEndpoint();
    }
}