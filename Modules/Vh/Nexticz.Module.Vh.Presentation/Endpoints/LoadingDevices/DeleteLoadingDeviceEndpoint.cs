using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadingDevices.Commands.DeleteLoadingDevice;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingDevices;

public static class DeleteLoadingDeviceEndpoint
{
    public static IEndpointRouteBuilder MapDeleteLoadingDevice(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.LoadingDevices.DeleteLoadingDevice,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteLoadingDeviceCommand { Id = id };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.LoadingDevices.DeleteLoadingDevice));

        return builder;
    }
}