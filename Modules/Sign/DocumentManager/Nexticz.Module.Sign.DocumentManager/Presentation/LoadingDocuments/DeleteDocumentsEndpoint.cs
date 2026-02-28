using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Orchestrators;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.LoadingDocuments;

internal static class DeleteDocumentsEndpoint
{
    public static IEndpointRouteBuilder MapDeleteDocumentsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(DocumentManagerEndpoints.LoadingDocumentEndpoints.DeleteDocuments,
                async (
                    DeleteDocumentsRequest request,
                    CancellationToken cancellationToken,
                    [FromServices] IDeleteDocumentsOrchestrator orchestrator) =>
                {
                    var fileResult = 
                        await orchestrator.OrchestrateAsync(request.DeleteReason, request.DeleteDocumentJobs, cancellationToken);
                    
                    return fileResult.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.LoadingDocumentEndpoints.DeleteDocuments)));

        return builder;
    }
}