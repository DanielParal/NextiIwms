using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Orchestrators;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.SigningDevices;

internal static class SendLoadingDocumentToSigningDeviceEndpoint
{
    public static IEndpointRouteBuilder MapSendLoadingDocumentToSigningDeviceEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(DocumentManagerEndpoints.SigningDeviceEndpoints.SendLoadingDocumentToSigningDevice,
                async (
                    string signingDeviceCode,
                    SendLoadingDocumentsToSigningDeviceRequest request,
                    CancellationToken cancellationToken,
                    [FromServices] IDeviceCommunicationOrchestrator orchestrator) =>
                {
                    var result = 
                        await orchestrator.SendDocumentsToDeviceAsync(
                            request.SendDocumentJobs, signingDeviceCode, 
                            request.DriverName, request.LicensePlate, cancellationToken);
                    
                    return result.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.SigningDeviceEndpoints.SendLoadingDocumentToSigningDevice)));

        return builder;
    }
}