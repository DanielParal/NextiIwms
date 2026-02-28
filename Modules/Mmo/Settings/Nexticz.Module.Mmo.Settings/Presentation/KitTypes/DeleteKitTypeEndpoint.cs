using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.DeleteKitType;

namespace Nexticz.Module.Mmo.Settings.Presentation.KitTypes;

internal static class DeleteKitTypeEndpoint
{
    public static IEndpointRouteBuilder MapDeleteKitType(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.KitTypeEndpoints.DeleteKitType,
                async (
                    string code, 
                    ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new DeleteKitTypeCommand(code);
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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitTypeEndpoints.DeleteKitType)));

        return builder;
    }
}