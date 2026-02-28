using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.SigningDevices;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.SigningDevices.Commands.UpdateSigningDevice;

namespace Nexticz.Module.Sign.Settings.Presentation.SigningDevices;

internal static class UpdateSigningDeviceEndpoint
{
    public static IEndpointRouteBuilder MapUpdateSigningDeviceEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.SigningDeviceEndpoints.UpdateSigningDevice,
                async (
                    string code, 
                    UpdateSigningDeviceRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(
                            new UpdateSigningDeviceCommand(code, request.Name, request.IsActive, request.LocationCode, request.PrinterCode), 
                            cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SigningDeviceEndpoints.UpdateSigningDevice)));
        
        return builder;
    }
}