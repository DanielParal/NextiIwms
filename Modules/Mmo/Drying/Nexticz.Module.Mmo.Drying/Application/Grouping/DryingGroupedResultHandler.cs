using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.SharedKernel.DevExtreme.Grouping;

namespace Nexticz.Module.Mmo.Drying.Application.Grouping;

internal class DryingGroupedResultHandler(ILogger<DryingGroupedResultHandler> logger,
    IDryingReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : GroupedResultHandler(logger, readOnlyEventStoreRepository), IDryingGroupedResultHandler;