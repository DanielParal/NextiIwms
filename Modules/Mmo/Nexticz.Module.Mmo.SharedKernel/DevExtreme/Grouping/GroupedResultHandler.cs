using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

namespace Nexticz.Module.Mmo.SharedKernel.DevExtreme.Grouping;

public class GroupedResultHandler(
    ILogger<GroupedResultHandler> logger,
    IReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : Lib.Shared.DevExtreme.Grouping.GroupedResultHandler(logger, readOnlyEventStoreRepository), IGroupedResultHandler;