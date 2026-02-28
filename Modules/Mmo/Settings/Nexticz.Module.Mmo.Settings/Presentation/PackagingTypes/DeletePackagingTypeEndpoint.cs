using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.DeletePackagingType;

namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingTypes;

internal static class DeletePackagingTypeEndpoint
{
    public static IEndpointRouteBuilder MapDeletePackagingType(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.PackagingTypeEndpoints.DeletePackagingType,
                async (
                    string code, 
                    ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new DeletePackagingTypeCommand(code);
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingTypeEndpoints.DeletePackagingType)));

        return builder;
    }
}