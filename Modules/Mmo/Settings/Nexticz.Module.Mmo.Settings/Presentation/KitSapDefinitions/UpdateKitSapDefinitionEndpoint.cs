using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.UpdateKitSapDefinition;


namespace Nexticz.Module.Mmo.Settings.Presentation.KitSapDefinitions;

internal static class UpdateKitSapDefinitionEndpoint
{
    public static IEndpointRouteBuilder MapUpdateKitSapDefinition(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.KitSapDefinitionEndpoints.UpdateKitSapDefinition,
                async (
                    string code, 
                    UpdateKitSapDefinitionRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateKitSapDefinitionCommand(code, request.Name);
                    var result = 
                        await sender.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitSapDefinitionEndpoints.UpdateKitSapDefinition)));
        
        return builder;
    }
}