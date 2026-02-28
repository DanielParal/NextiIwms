using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Imports;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.FileHandling;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.Commands.CreateImport;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.Imports;

internal static class CreateImportEndpoint
{
    public static IEndpointRouteBuilder MapCreateImport(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.ImportEndpoints.CreateImport,
                async (
                    HttpRequest request,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsFileHandler fileHandler) =>
                {
                    var fileResult = await fileHandler.GetFileFromHttpRequestAsync(request, cancellationToken);
                    if (fileResult.IsError)
                        return ResultsHelper.Problem(fileResult.Errors);
                    
                    var form = await request.ReadFormAsync(cancellationToken);
                    var processedRequestSuccessful = UploadImportFormRequest.TryGetRequestFromFormCollection(form, out var uploadImportRequest);
                    if (!processedRequestSuccessful || uploadImportRequest is null)
                        return Results.BadRequest();

                    var result =
                        await sender.Send(
                            new CreateImportCommand((ImportType)uploadImportRequest.ImportType, fileResult.Value), 
                            cancellationToken);
                    
                    return result.Match(
                        import => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ImportEndpoints.CreateImport)))
            .Accepts<UploadImportFormRequest>("multipart/form-data");

        return builder;
    }
}