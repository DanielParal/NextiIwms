using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailTemplates;

internal static class EmailTemplateEndpointsExtensions
{
    public static IEndpointRouteBuilder MapEmailTemplatesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetEmailTemplatesEndpoint()
            .MapUpdateEmailTemplateEndpoint()
            .MapGetEmailTemplateByIdEndpoint();
    }
    
    public static IEndpointRouteBuilder MapDeveloperOnlyEmailTemplatesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateEmailTemplateEndpoint()
            .MapDeleteEmailTemplateEndpoint();
    }
}