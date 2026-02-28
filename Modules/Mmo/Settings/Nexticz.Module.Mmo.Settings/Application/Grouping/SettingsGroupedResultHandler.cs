using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.SharedKernel.DevExtreme.Grouping;

namespace Nexticz.Module.Mmo.Settings.Application.Grouping;

internal class SettingsGroupedResultHandler(ILogger<SettingsGroupedResultHandler> logger,
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : GroupedResultHandler(logger, readOnlyEventStoreRepository), ISettingsGroupedResultHandler;