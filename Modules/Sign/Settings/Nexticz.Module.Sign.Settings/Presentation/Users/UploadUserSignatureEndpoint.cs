using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.FileHandling;
using Nexticz.Module.Sign.Settings.Application.Users.Commands.UploadUserSignature;

namespace Nexticz.Module.Sign.Settings.Presentation.Users;

internal static class UploadUserSignatureEndpoint
{
    public static IEndpointRouteBuilder MapUploadUserSignatureEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.UserEndpoints.UploadSignature,
                async (
                    string userName,
                    HttpRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsFileHandler settingsFileHandler) =>
                {
                    var fileResult = await settingsFileHandler.GetFileFromHttpRequestAsync(request, cancellationToken);
                    if (fileResult.IsError)
                        return ResultsHelper.Problem(fileResult.Errors);
                    
                    var uploadResult = 
                        await sender.Send(new UploadUserSignatureCommand(userName, fileResult.Value), cancellationToken);
                    
                    return uploadResult.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.UserEndpoints.UploadSignature)))
            .Accepts<IFormCollection>("multipart/form-data");

        return builder;
    }
}