using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Orchestrators;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.LoadingDocuments;

internal static class DownloadSignedDocumentsEndpoint
{
    public static IEndpointRouteBuilder MapDownloadSignedDocumentsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(DocumentManagerEndpoints.LoadingDocumentEndpoints.DownloadSignedDocuments,
                async (
                    DownloadDocumentsRequest request,
                    CancellationToken cancellationToken,
                    [FromServices] IDownloadSignedDocumentsOrchestrator orchestrator) =>
                {
                    var fileResult = 
                        await orchestrator.OrchestrateAsync(request.DownloadDocumentJobs, cancellationToken);
                    
                    return fileResult.Match(
                        file => 
                            Results.Ok(new FileResponse(file.ContentBytes, file.ContentType, file.FileName)),
                        ResultsHelper.Problem);
                })
            .Produces<FileResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.LoadingDocumentEndpoints.DownloadSignedDocuments)));

        return builder;
    }
}