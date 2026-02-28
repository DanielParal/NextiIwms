using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Contracts.UnsignedLoadingDocuments;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.DocumentManager.Application.Grouping;
using Nexticz.Module.Sign.DocumentManager.Application.UnsignedLoadingDocuments.Queries.GetUnsignedLoadingDocumentsForUser;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.UnsignedLoadingDocuments;

internal static class GetUnsignedLoadingDocumentsEndpoint
{
    public static IEndpointRouteBuilder MapGetUnsingedLoadingDocumentsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(DocumentManagerEndpoints.UnsignedLoadingDocumentEndpoints.GetUnsignedLoadingDocuments,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] IDocumentManagerGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<LoadingDocument>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetUnsignedLoadingDocumentsForUserQuery(filteringParams), cancellationToken);
                    
                    var mappedResultAndSorted =
                        result.MapDataFromTInToTOut(UnsignedLoadingDocumentResponseFactory.Create);
                    mappedResultAndSorted.Data = mappedResultAndSorted.Data.OrderByDescending(x => x.CreatedAt).ToList();
                    
                    return Results.Ok(mappedResultAndSorted);
                })
            .Produces<FilteredResult<UnsignedLoadingDocumentResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.UnsignedLoadingDocumentEndpoints.GetUnsignedLoadingDocuments)));

        return builder;
    }
}