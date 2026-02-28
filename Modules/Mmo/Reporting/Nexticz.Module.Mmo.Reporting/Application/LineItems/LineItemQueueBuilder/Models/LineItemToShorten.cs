using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder.Models;

internal record LineItemToShorten(Guid LineItemId, DateTimeOffset StartDate, DateTimeOffset EndDate, LineItemType Type);