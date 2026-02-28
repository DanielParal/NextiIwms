using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Application.Kits;
using Nexticz.Module.Mmo.Settings.Application.Kits.Commands.UploadKitInstruction;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Kits.Commands.DeleteKitInstruction;


namespace Nexticz.Module.Mmo.Settings.Presentation.Kits;

internal static class DeleteKitInstructionEndpoint
{
    public static IEndpointRouteBuilder MapDeleteKitInstructionEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.KitEndpoints.DeleteKitInstruction,
                async (
                    string code,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new DeleteKitInstructionCommand(code), cancellationToken);
                    
                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitEndpoints.DeleteKitInstruction)));

        return builder;
    }
}