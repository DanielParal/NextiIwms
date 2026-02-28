using Marten;
using MediatR;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentsByCodes;

internal class GetLoadingDocumentsByCodesQueryHandler(
    IDocumentManagerReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetLoadingDocumentsByCodesQuery, LoadingDocument[]>
{
    public async Task<LoadingDocument[]> Handle(GetLoadingDocumentsByCodesQuery request, CancellationToken cancellationToken)
    {
        var upperCodes = request.LoadingDocumentCodes.Select(x => x.ToUpperInvariant()).ToArray();
        var loadingDocuments = await readOnlyEventStoreRepository.GetAllByConditionAsync<LoadingDocument>(
            x => x.Code.IsOneOf(upperCodes), cancellationToken);
        
        return loadingDocuments.ToArray();
    }
}