using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadingDevices.Commands.CreateLoadingDevice;
using Nexticz.Module.Vh.Contracts.LoadingDevices;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingDevices;

public static class CreateLoadingDeviceEndpoint
{
    public static IEndpointRouteBuilder MapCreateLoadingDevice(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.LoadingDevices.CreateLoadingDevice,
                async (CreateLoadingDeviceRequest createLoadingDeviceRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateLoadingDeviceCommand { CreateLoadingDeviceRequest = createLoadingDeviceRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.LoadingDevices.CreateLoadingDevice));

        return builder;
    }
}