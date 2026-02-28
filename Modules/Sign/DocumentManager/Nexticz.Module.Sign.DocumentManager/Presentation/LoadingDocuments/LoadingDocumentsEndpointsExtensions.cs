using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.LoadingDocuments;

internal static class LoadingDocumentsEndpointsExtensions
{
    public static IEndpointRouteBuilder MapLoadingDocumentsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapManuallySignDocumentEndpoint()
            .MapChangePrintCopiesCountEndpoint()
            .MapDownloadSignedDocumentsEndpoint();
    }
    
    public static IEndpointRouteBuilder MapDeleteLoadingDocumentsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapDeleteDocumentsEndpoint();
    }
}