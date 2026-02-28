using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Locations;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Locations.Queries.GetLocationByCode;

namespace Nexticz.Module.Sign.Settings.Presentation.Locations;

internal static class GetLocationByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetLocationByCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.LocationEndpoints.GetLocationByCode,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetLocationByCodeQuery(code), cancellationToken);
                    
                    return result.Match(
                        depositor => Results.Ok(LocationResponseFactory.Create(depositor)),
                        ResultsHelper.Problem);
                })
            .Produces<LocationResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.LocationEndpoints.GetLocationByCode)));

        return builder;
    }
}