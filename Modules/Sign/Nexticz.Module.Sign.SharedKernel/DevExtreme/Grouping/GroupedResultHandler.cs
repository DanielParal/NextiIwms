using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.SharedKernel.DataAccess;

namespace Nexticz.Module.Sign.SharedKernel.DevExtreme.Grouping;

public class GroupedResultHandler(
    ILogger<GroupedResultHandler> logger,
    IReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : Lib.Shared.DevExtreme.Grouping.GroupedResultHandler(logger, readOnlyEventStoreRepository), IGroupedResultHandler;