using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Application.SigningDevices;
using Nexticz.Module.Sign.Settings.Contracts.SigningDevices;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.SigningDevices.Commands.CreateSigningDevice;

namespace Nexticz.Module.Sign.Settings.Presentation.SigningDevices;

internal static class CreateSigningDeviceEndpoint
{
    public static IEndpointRouteBuilder MapCreateSigningDeviceEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.SigningDeviceEndpoints.CreateSigningDevice,
                async (
                    CreateSigningDeviceRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(
                            new CreateSigningDeviceCommand(
                                request.Code, request.Name, request.IsActive, request.LocationCode, request.PrinterCode), 
                            cancellationToken);

                    return result.Match(
                        signingDevice => Results.Created(
                            $"{SettingsEndpoints.SigningDeviceEndpoints.CreateSigningDevice}/{signingDevice.Code}",
                            SigningDeviceResponseFactory.Create(signingDevice)),
                        ResultsHelper.Problem);
                })
            .Produces<SigningDeviceResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SigningDeviceEndpoints.CreateSigningDevice)));

        return builder;
    }
}