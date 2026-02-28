using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Commands.AddItems;

public record AddItemsCommand(Guid ShiftId, DateTimeOffset StartDate, DateTimeOffset EndDate, AddLineItemType Type, string[] LineCodes) : IReportingCommand<ErrorOr<Success>>;