using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadingDevices.Queries.GetLaodingDeviceByDeviceKey;
using Nexticz.Module.Vh.Contracts.LoadingDevices;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingDevices;

public static class GetLoadingDeviceByDeviceKeyEndpoint
{
    public static IEndpointRouteBuilder MapGetLoadingDeviceByDeviceKey(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.LoadingDevices.GetLoadingDeviceByDeviceKey,
                async (Guid deviceKey, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetLoadingDeviceByDeviceKeyQuery { DeviceKey = deviceKey };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<LoadingDeviceResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .AllowAnonymous()
            .WithName(nameof(ApiEndpoints.LoadingDevices.GetLoadingDeviceByDeviceKey));

        return builder;
    }
}