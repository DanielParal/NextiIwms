using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentByCode;

internal class GetLoadingDocumentByCodeQueryHandler(
    IDocumentManagerReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetLoadingDocumentByCodeQuery, ErrorOr<LoadingDocument>>
{
    public async Task<ErrorOr<LoadingDocument>> Handle(GetLoadingDocumentByCodeQuery request, CancellationToken cancellationToken)
    {
        var loadingDocument = await readOnlyEventStoreRepository.GetFirstByConditionAsync<LoadingDocument>(
            x => x.Code == request.LoadingDocumentCode, cancellationToken);
        
        if (loadingDocument is null)
            return LoadingDocumentErrors.LoadingDocumentNotFound;
        
        return loadingDocument;
    }
}