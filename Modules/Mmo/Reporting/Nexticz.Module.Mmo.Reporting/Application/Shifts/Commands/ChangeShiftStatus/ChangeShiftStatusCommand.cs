using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.ChangeShiftStatus;

internal record ChangeShiftStatusCommand(Guid ShiftId, ShiftStatus FromStatus, ShiftStatus ToStatus) : IReportingCommand<Success>;