using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.ManuallySignDocument;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.LoadingDocuments;

internal static class ManuallySignDocumentEndpoint
{
    public static IEndpointRouteBuilder MapManuallySignDocumentEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(DocumentManagerEndpoints.LoadingDocumentEndpoints.ManuallySignDocument,
                async (
                    string loadingDocumentCode,
                    HttpRequest request,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] IDocumentManagerFileHandler fileHandler) =>
                {
                    var fileResult = await fileHandler.GetFileFromHttpRequestAsync(request, cancellationToken);
                    if (fileResult.IsError)
                        return ResultsHelper.Problem(fileResult.Errors);
                    
                    var form = await request.ReadFormAsync(cancellationToken);
                    var processedRequestSuccessful = 
                        ManuallySignDocumentFormResponse.TryGetRequestFromFormCollection(form, out var manuallySignDocumentRequest);
                    
                    if (!processedRequestSuccessful)
                        return Results.BadRequest();

                    var result = 
                        await sender.Send(new ManuallySignDocumentCommand(
                                loadingDocumentCode, manuallySignDocumentRequest?.DeliveryDocumentCode, fileResult.Value), 
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
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.LoadingDocumentEndpoints.ManuallySignDocument)))
            .Accepts<ManuallySignDocumentFormResponse>("multipart/form-data");

        return builder;
    }
}