using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.SigningDevices;

internal static class SigningDeviceEndpointsExtensions
{
    public static IEndpointRouteBuilder MapSharedSigningDevicesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetSigningDevicesEndpoint();
    }
    
    public static IEndpointRouteBuilder MapSigningDevicesForUserEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapSendLoadingDocumentToSigningDeviceEndpoint()
            .MapReturnLoadingDocumentFromSigningDeviceEndpoint();
    }
    
    public static IEndpointRouteBuilder MapSigningDevicesForDeviceEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetDocumentsForSigningDeviceEndpoint()
            .MapGetDocumentFileForSigningDeviceEndpoint()
            .MapSignDocumentsEndpoint();
    }
}