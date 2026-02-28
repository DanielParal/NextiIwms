using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.Imports;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Cuzk.Application.FileHandling;
using Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Commands.RequestImport;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Presentation.Imports;

internal static class RequestImportEndpoint
{
    public static IEndpointRouteBuilder MapRequestImport(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(CuzkEndpoints.ImportEndpoints.RequestImport,
                async (
                    HttpRequest request,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ICuzkFileHandler fileHandler) =>
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
                            new RequestImportCommand((ImportType)uploadImportRequest.ImportType, fileResult.Value, uploadImportRequest.CsvDelimiter), 
                            cancellationToken);
                    
                    return result.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.ImportEndpoints.RequestImport)))
            .Accepts<UploadImportFormRequest>("multipart/form-data")
            .WithMetadata(new RequestSizeLimitAttribute(200_000_000))
            .WithMetadata(new RequestFormLimitsAttribute
            {
                MultipartBodyLengthLimit = 200_000_000
            });

        return builder;
    }
}