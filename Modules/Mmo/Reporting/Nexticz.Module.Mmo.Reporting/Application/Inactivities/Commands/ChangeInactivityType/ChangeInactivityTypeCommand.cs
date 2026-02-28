using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.ChangeInactivityType;

internal record ChangeInactivityTypeCommand(Guid Id, InactivityType Type, Guid? InactivityReasonId) : IReportingCommand<ErrorOr<Success>>;