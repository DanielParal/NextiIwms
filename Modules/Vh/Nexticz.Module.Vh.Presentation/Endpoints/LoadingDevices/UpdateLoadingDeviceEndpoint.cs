using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadingDevices.Commands.UpdateLoadingDevice;
using Nexticz.Module.Vh.Contracts.LoadingDevices;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingDevices;

public static class UpdateLoadingDeviceEndpoint
{
    public static IEndpointRouteBuilder MapUpdateLoadingDevice(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.LoadingDevices.UpdateLoadingDevice,
                async (Guid deviceKey, UpdateLoadingDeviceRequest updateWorkerRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateLoadingDeviceCommand
                        { DeviceKey = deviceKey, UpdateLoadingDeviceRequest = updateWorkerRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.LoadingDevices.UpdateLoadingDevice));

        return builder;
    }
}