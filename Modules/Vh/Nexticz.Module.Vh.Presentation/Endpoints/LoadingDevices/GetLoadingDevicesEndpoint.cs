using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadingDevices.Common.Models;
using Nexticz.Module.Vh.Application.LoadingDevices.Queries.GetLoadingDevices;
using Nexticz.Module.Vh.Contracts.LoadingDevices;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingDevices;

public static class GetLoadingDevicesEndpoint
{
    public static IEndpointRouteBuilder MapGetLoadingDevices(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.LoadingDevices.GetLoadingDevices,
                async ([AsParameters] LoadingDevicesFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetLoadingDevicesQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<LoadingDeviceResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.LoadingDevices.GetLoadingDevices));

        return builder;
    }
}