using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.DeletePackagingCirculation;

namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingCirculations;

internal static class DeletePackagingCirculationEndpoint
{
    public static IEndpointRouteBuilder MapDeletePackagingCirculation(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.PackagingCirculationEndpoints.DeletePackagingCirculation,
                async (
                    string code,
                    ISender mediator, 
                    CancellationToken cancellationToken) =>
                {
                    var command = new DeletePackagingCirculationCommand(code);
                    
                    var result = await mediator.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingCirculationEndpoints.DeletePackagingCirculation)));

        return builder;
    }
}