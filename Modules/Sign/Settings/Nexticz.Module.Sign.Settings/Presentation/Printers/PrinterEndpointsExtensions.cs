using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.Printers;

internal static class PrinterEndpointsExtensions
{
    public static IEndpointRouteBuilder MapPrintersEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreatePrinterEndpoint()
            .MapGetPrinterByCodeEndpoint()
            .MapGetPrintersEndpoint()
            .MapUpdatePrinterEndpoint()
            .MapDeletePrinterEndpoint();
    }
}