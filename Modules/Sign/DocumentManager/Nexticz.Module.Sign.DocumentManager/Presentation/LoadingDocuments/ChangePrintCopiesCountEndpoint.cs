using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.ChangePrintCopiesCount;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.LoadingDocuments;

internal static class ChangePrintCopiesCountEndpoint
{
    public static IEndpointRouteBuilder MapChangePrintCopiesCountEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(DocumentManagerEndpoints.LoadingDocumentEndpoints.ChangePrintCopiesCount,
                async (
                    string loadingDocumentCode,
                    ChangePrintCopiesCountRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new ChangePrintCopiesCountCommand(
                                loadingDocumentCode, request.DeliveryDocumentCode, request.PrintCopiesCount), 
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
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.LoadingDocumentEndpoints.ChangePrintCopiesCount)));

        return builder;
    }
}