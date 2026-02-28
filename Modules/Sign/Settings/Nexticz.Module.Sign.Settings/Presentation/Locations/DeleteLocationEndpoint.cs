using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Locations.Commands.DeleteLocation;

namespace Nexticz.Module.Sign.Settings.Presentation.Locations;

internal static class DeleteLocationEndpoint
{
    public static IEndpointRouteBuilder MapDeleteLocationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.LocationEndpoints.DeleteLocation,
                async (
                    string code, 
                    ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var result = await mediatr.Send(new DeleteLocationCommand(code), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.LocationEndpoints.DeleteLocation)));

        return builder;
    }
}