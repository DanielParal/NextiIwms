using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.DocumentTemplates;

internal static class DocumentTemplateEndpointsExtensions
{
    public static IEndpointRouteBuilder MapDocumentTemplatesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetDocumentTemplatesEndpoint()
            .MapGetDocumentTemplateByCodeEndpoint();
    }
    
    public static IEndpointRouteBuilder MapDeveloperOnlyDocumentTemplatesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateDocumentTemplateEndpoint()
            .MapUpdateDocumentTemplateEndpoint()
            .MapDeleteDocumentTemplateEndpoint();
    }
}