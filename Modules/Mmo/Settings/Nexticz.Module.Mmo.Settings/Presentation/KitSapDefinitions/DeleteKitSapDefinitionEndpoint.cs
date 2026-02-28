using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.DeleteKitSapDefinition;

namespace Nexticz.Module.Mmo.Settings.Presentation.KitSapDefinitions;

internal static class DeleteKitSapDefinitionEndpoint
{
    public static IEndpointRouteBuilder MapDeleteKitSapDefinition(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.KitSapDefinitionEndpoints.DeleteKitSapDefinition,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var command = new DeleteKitSapDefinitionCommand(code);
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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitSapDefinitionEndpoints.DeleteKitSapDefinition)));

        return builder;
    }
}