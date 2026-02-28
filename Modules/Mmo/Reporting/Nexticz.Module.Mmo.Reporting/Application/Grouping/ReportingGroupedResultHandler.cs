using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.SharedKernel.DevExtreme.Grouping;

namespace Nexticz.Module.Mmo.Reporting.Application.Grouping;

internal class ReportingGroupedResultHandler(ILogger<ReportingGroupedResultHandler> logger,
    IReportingReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : GroupedResultHandler(logger, readOnlyEventStoreRepository), IReportingGroupedResultHandler;