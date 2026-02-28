using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Locations;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Locations.Commands.CreateLocation;


namespace Nexticz.Module.Sign.Settings.Presentation.Locations;

internal static class CreateLocationEndpoint
{
    public static IEndpointRouteBuilder MapCreateLocationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.LocationEndpoints.CreateLocation,
                async (
                    CreateLocationRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreateLocationCommand(request.Code, request.Name), cancellationToken);

                    return result.Match(
                        location => Results.Created(
                            $"{SettingsEndpoints.LocationEndpoints.GetLocations}/{location.Code}",
                            LocationResponseFactory.Create(location)),
                        ResultsHelper.Problem);
                })
            .Produces<LocationResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.LocationEndpoints.CreateLocation)));

        return builder;
    }
}