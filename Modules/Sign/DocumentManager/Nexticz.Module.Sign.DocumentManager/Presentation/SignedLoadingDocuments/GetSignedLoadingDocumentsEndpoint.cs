using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Contracts.SignedLoadingDocuments;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.DocumentManager.Application.Grouping;
using Nexticz.Module.Sign.DocumentManager.Application.SignedLoadingDocuments;
using Nexticz.Module.Sign.DocumentManager.Application.SignedLoadingDocuments.Queries.GetSignedLoadingDocumentsForUser;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.SignedLoadingDocuments;

internal static class GetSignedLoadingDocumentsEndpoint
{
    public static IEndpointRouteBuilder MapGetSingedLoadingDocumentsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(DocumentManagerEndpoints.SignedLoadingDocumentEndpoints.GetSignedLoadingDocuments,
                async (
                    [AsParameters] SignedLoadingDocumentFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] IDocumentManagerGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<LoadingDocument>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetSignedLoadingDocumentsForUserQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(SignedLoadingDocumentResponseFactory.Create));
                })
            .Produces<FilteredResult<SignedLoadingDocumentResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.SignedLoadingDocumentEndpoints.GetSignedLoadingDocuments)));

        return builder;
    }
}