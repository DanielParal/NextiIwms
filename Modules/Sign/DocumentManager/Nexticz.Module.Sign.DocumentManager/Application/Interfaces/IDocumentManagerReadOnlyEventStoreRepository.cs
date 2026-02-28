
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Application.Interfaces;

internal interface IDocumentManagerReadOnlyEventStoreRepository : Module.Sign.SharedKernel.DataAccess.IReadOnlyEventStoreRepository
{
    Task<FilteredResult<UnsignedLoadingDocumentView>> GetUnsignedLoadingDocumentsForUserAsync(
        BaseFilteringParams filteringParams,
        string[] depositorCodes,
        CancellationToken cancellationToken);
    
    Task<FilteredResult<SignedLoadingDocumentView>> GetSignedLoadingDocumentsForUserAsync(
        BaseFilteringParams filteringParams,
        string[] depositorCodes,
        string? partnersOrderNumber,
        string? rznoCode,
        string? combinedRznoCode,
        string? partnerName,
        string? receiverName, 
        string? fullTextFilter,
        CancellationToken cancellationToken);
}