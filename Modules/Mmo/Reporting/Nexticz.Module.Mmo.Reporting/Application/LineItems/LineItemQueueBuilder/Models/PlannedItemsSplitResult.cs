using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder.Models;

internal record PlannedItemsSplitResult(List<LineItemToCreateOrUpdate> NewLineItems, List<LineItemView> PlannedItemsToRemove, DateTimeOffset LastEndDate);