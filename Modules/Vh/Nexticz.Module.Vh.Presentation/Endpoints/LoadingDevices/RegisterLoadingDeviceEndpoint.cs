using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadingDevices.Commands.RegisterLoadingDevice;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingDevices;

public static class RegisterLoadingDeviceEndpoint
{
    public static IEndpointRouteBuilder MapRegisterLoadingDevice(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.LoadingDevices.RegisterLoadingDevice,
            async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
            {
                var command = new RegisterLoadingDeviceCommand { DeviceKey = id };
                var result = await mediatr.Send(command, cancellationToken);

                return result.Match(
                    _ => Results.NoContent(),
                    ResultsHelper.Problem);
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0)
            .AllowAnonymous()
            .WithName(nameof(ApiEndpoints.LoadingDevices.RegisterLoadingDevice));
        
        return builder;
    }
}