using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.SigningDevices;

internal static class SigningDeviceEndpointsExtensions
{
    public static IEndpointRouteBuilder MapSigningDevicesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateSigningDeviceEndpoint()
            .MapGetSigningDeviceByCodeEndpoint()
            .MapGetSigningDevicesEndpoint()
            .MapUpdateSigningDeviceEndpoint()
            .MapDeleteSigningDeviceEndpoint();
    }
}