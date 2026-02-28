using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder.Models;

internal record NextToLastShiftBuilderResult(List<LineItemToCreateOrUpdate> LineItemsToCreateOrUpdate, List<LineItemView> PlannedItemsToRemove, DateTimeOffset LastItemOnLineEndDate);