using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.SharedKernel.DevExtreme.Grouping;

namespace Nexticz.Module.Sign.Settings.Application.Grouping;

internal class SettingsGroupedResultHandler(ILogger<SettingsGroupedResultHandler> logger,
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : GroupedResultHandler(logger, readOnlyEventStoreRepository), ISettingsGroupedResultHandler;