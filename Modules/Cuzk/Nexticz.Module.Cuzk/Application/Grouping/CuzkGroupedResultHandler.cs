using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.DevExtreme.Grouping;
using Nexticz.Module.Cuzk.Application.Interfaces;

namespace Nexticz.Module.Cuzk.Application.Grouping;

internal class CuzkGroupedResultHandler(ILogger<CuzkGroupedResultHandler> logger,
    ICuzkReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : GroupedResultHandler(logger, readOnlyEventStoreRepository), ICuzkGroupedResultHandler;