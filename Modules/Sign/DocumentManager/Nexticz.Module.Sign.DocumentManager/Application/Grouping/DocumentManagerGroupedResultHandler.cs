using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.SharedKernel.DevExtreme.Grouping;

namespace Nexticz.Module.Sign.DocumentManager.Application.Grouping;

internal class DocumentManagerGroupedResultHandler(ILogger<DocumentManagerGroupedResultHandler> logger,
    IDocumentManagerReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : GroupedResultHandler(logger, readOnlyEventStoreRepository), IDocumentManagerGroupedResultHandler;