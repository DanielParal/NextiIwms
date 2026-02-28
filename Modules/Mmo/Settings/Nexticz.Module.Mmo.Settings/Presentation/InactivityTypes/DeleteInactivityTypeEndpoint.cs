using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Commands.DeleteInactivityType;

namespace Nexticz.Module.Mmo.Settings.Presentation.InactivityTypes;

internal static class DeleteInactivityTypeEndpoint
{
    public static IEndpointRouteBuilder MapDeleteInactivityTypeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.InactivityTypeEndpoints.DeleteInactivityType,
                async (
                    Guid id,
                    ISender sender, 
                    CancellationToken cancellationToken) =>
                {
                    var command = new DeleteInactivityTypeCommand(id);
                    var result = await sender.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.InactivityTypeEndpoints.DeleteInactivityType)));

        return builder;
    }
}