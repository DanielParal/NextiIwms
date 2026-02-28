
using ErrorOr;

namespace Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.UnplanInactivity;

internal record UnplanInactivityCommand(Guid Id) : IReportingCommand<ErrorOr<Success>>;