using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Application.Kits;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;
using Nexticz.Module.Mmo.Settings.Application.Kits.Commands.UploadKitInstruction;


namespace Nexticz.Module.Mmo.Settings.Presentation.Kits;

internal static class UploadKitInstructionEndpoint
{
    public static IEndpointRouteBuilder MapUploadKitInstructionEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.KitEndpoints.UploadKitInstruction,
                async (
                    string code,
                    HttpRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsFileHandler fileHandler) =>
                {
                    var fileResult = await fileHandler.GetFileFromHttpRequestAsync(request, cancellationToken);
                    if (fileResult.IsError)
                        return ResultsHelper.Problem(fileResult.Errors);
                    
                    var uploadResult = 
                        await sender.Send(new UploadKitInstructionCommand(code, fileResult.Value), cancellationToken);
                    
                    return uploadResult.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitEndpoints.UploadKitInstruction)))
            .Accepts<IFormCollection>("multipart/form-data");

        return builder;
    }
}