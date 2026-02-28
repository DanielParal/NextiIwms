using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Application.SigningDevices;
using Nexticz.Module.Sign.Settings.Contracts.SigningDevices;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Grouping;
using Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevices;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.SigningDevices;

internal static class GetSigningDevicesEndpoint
{
    public static IEndpointRouteBuilder MapGetSigningDevicesEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.SigningDeviceEndpoints.GetSigningDevices,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<SigningDevice>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetSigningDevicesQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(SigningDeviceResponseFactory.Create));
                })
            .Produces<FilteredResult<SigningDeviceResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SigningDeviceEndpoints.GetSigningDevices)));

        return builder;
    }
}