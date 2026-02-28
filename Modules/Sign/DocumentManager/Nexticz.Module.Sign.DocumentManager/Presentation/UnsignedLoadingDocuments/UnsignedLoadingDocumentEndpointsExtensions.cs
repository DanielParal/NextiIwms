using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Presentation.SigningDevices;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.UnsignedLoadingDocuments;

internal static class UnsignedLoadingDocumentEndpointsExtensions
{
    public static IEndpointRouteBuilder MapUnsignedLoadingDocumentsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetUnsingedLoadingDocumentsEndpoint();
    }
}