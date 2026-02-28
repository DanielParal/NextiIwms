using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetDocumentFileForSigningDevice;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.SigningDevices;

internal static class GetDocumentFileForSigningDeviceEndpoint
{
    public static IEndpointRouteBuilder MapGetDocumentFileForSigningDeviceEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(DocumentManagerEndpoints.SigningDeviceEndpoints.GetDocumentFileForSigningDevice,
                async (
                    string signingDeviceCode,
                    string documentCode,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new GetDocumentFileForSigningDeviceQuery(signingDeviceCode, documentCode), cancellationToken);
                    
                    return result.Match(
                        fileResult => 
                            Results.Ok(new FileResponse(fileResult.ContentBytes, fileResult.ContentType, fileResult.FileName)),
                        ResultsHelper.Problem);
                })
            .Produces<FileResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.SigningDeviceEndpoints.GetDocumentFileForSigningDevice)));

        return builder;
    }
}