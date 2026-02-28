using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetDocumentsForSigningDeviceResponse;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.SigningDevices;

internal static class GetDocumentsForSigningDeviceEndpoint
{
    public static IEndpointRouteBuilder MapGetDocumentsForSigningDeviceEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(DocumentManagerEndpoints.SigningDeviceEndpoints.GetDocumentsForSigningDevice,
                async (
                    string signingDeviceCode,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new GetDocumentsForSigningDeviceResponseQuery(signingDeviceCode), cancellationToken);
                    
                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<DocumentsForSigningDeviceResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.SigningDeviceEndpoints.GetDocumentsForSigningDevice)));

        return builder;
    }
}