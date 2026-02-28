using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.DevExtreme.Grouping;
using Nexticz.Module.Portal.Application.Interfaces;

namespace Nexticz.Module.Portal.Application.Grouping;

internal class PortalGroupedResultHandler(ILogger<PortalGroupedResultHandler> logger,
    IPortalReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : GroupedResultHandler(logger, readOnlyEventStoreRepository), IPortalGroupedResultHandler;