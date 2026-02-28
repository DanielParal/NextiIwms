using ErrorOr;

namespace Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.ChangeInactivityTimeInterval;

internal record ChangeInactivityTimeIntervalCommand(Guid Id, DateTimeOffset StartDate, DateTimeOffset EndDate) : IReportingCommand<ErrorOr<Success>>;