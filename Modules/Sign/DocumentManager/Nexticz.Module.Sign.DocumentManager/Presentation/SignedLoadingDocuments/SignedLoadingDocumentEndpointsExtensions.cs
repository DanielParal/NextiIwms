using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.SignedLoadingDocuments;

internal static class SignedLoadingDocumentEndpointsExtensions
{
    public static IEndpointRouteBuilder MapSignedLoadingDocumentsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetSingedLoadingDocumentsEndpoint();
    }
}