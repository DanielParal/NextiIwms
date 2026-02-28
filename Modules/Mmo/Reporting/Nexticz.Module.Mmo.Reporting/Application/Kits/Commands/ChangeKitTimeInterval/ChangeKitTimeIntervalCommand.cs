
using ErrorOr;

namespace Nexticz.Module.Mmo.Reporting.Application.Kits.Commands.ChangeKitTimeInterval;

internal record ChangeKitTimeIntervalCommand(Guid Id, DateTimeOffset StartDate, DateTimeOffset EndDate) : IReportingCommand<ErrorOr<Success>>;