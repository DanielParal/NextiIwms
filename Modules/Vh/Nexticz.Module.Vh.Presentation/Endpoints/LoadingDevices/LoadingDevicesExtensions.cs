using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingDevices;

public static class LoadingDevicesExtensions
{
    public static IEndpointRouteBuilder MapLoadingDevicesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateLoadingDevice()
            .MapUpdateLoadingDevice()
            .MapDeleteLoadingDevice()
            .MapGetLoadingDeviceByDeviceKey()
            .MapGetLoadingDevices()
            .MapRegisterLoadingDevice();
    }
}