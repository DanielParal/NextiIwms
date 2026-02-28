using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Orchestrators;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.SigningDevices;

internal static class ReturnLoadingDocumentFromSigningDeviceEndpoint
{
    public static IEndpointRouteBuilder MapReturnLoadingDocumentFromSigningDeviceEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(DocumentManagerEndpoints.SigningDeviceEndpoints.ReturnLoadingDocumentFromSigningDevice,
                async (
                    string signingDeviceCode,
                    CancellationToken cancellationToken,
                    [FromServices] IDeviceCommunicationOrchestrator orchestrator) =>
                {
                    var result = 
                        await orchestrator.ReturnDocumentsFromDeviceAsync(signingDeviceCode, cancellationToken);
                    
                    return result.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.SigningDeviceEndpoints.ReturnLoadingDocumentFromSigningDevice)));

        return builder;
    }
}