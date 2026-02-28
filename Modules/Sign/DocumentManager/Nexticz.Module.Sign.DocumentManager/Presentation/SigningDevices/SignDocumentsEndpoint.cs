using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SigningPublishers;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.SigningDevices;

internal static class SignDocumentsEndpoint
{
    private class SignDocumentsEndpointLogCategory;
    
    public static IEndpointRouteBuilder MapSignDocumentsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(DocumentManagerEndpoints.SigningDeviceEndpoints.SignDocuments,
                async (
                    string signingDeviceCode,
                    HttpRequest request,
                    CancellationToken cancellationToken,
                    [FromServices] ILogger<SignDocumentsEndpointLogCategory> logger, 
                    [FromServices] IDocumentManagerFileHandler fileHandler,
                    [FromServices] IDocumentManagerSigningPublisher signingPublisher,
                    [FromServices] ICurrentUserProvider currentUserProvider) =>
                {
                    var fileResult = await fileHandler.GetFileFromHttpRequestAsync(request, cancellationToken);
                    if (fileResult.IsError)
                    {
                        logger.LogWarning(
                            "SIGN - SignDocumentsEndpoint - cannot get file from request. Cannot sign documents. Signing device code: {SigningDeviceCode}",
                            signingDeviceCode);
                        return ResultsHelper.Problem(fileResult.Errors);
                    }
                    
                    var form = await request.ReadFormAsync(cancellationToken);
                    var processedRequestSuccessful = SignDocumentsFormRequest.TryGetRequestFromFormCollection(form, out var signDocumentsRequest);
                    if (!processedRequestSuccessful)
                    {
                        logger.LogWarning(
                            "SIGN - SignDocumentsEndpoint - cannot get request from form collection. Cannot sign documents. Signing device code: {SigningDeviceCode}",
                            signingDeviceCode);
                        return Results.BadRequest();
                    }
                    
                    var isFileValid = fileHandler.IsFileValid(fileResult.Value, 5* 1024 * 1024, [".jpg", ".jpeg", ".png"]);
                    if (isFileValid.IsError)
                    {
                        logger.LogWarning(
                            "SIGN - SignDocumentsEndpoint - file is not valid. Cannot sign documents. Signing device code: {SigningDeviceCode}, " +
                            ", file error code: {DomainErrorCode}, file error message: {DomainErrorMessage}.",
                            signingDeviceCode, isFileValid.FirstError.Code, isFileValid.FirstError.Description);
                        return ResultsHelper.Problem(isFileValid.Errors);
                    }
                    
                    await signingPublisher.PublishDocumentsSigningAsync(
                        signingDeviceCode, 
                        currentUserProvider.GetCurrentUser().UserName, 
                        signDocumentsRequest!.DriverName, 
                        signDocumentsRequest.LicensePlate, 
                        fileResult.Value, 
                        cancellationToken);
                    
                    return Results.Accepted();
                })
            .Produces<SignDocumentsResponse>()
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.SigningDeviceEndpoints.SignDocuments)))
            .Accepts<SignDocumentsFormRequest>("multipart/form-data");

        return builder;
    }
}