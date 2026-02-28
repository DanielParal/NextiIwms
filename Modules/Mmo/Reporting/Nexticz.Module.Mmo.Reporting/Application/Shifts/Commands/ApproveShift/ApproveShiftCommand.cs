using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.ApproveShift;

internal record ApproveShiftCommand(Guid ShiftId) : IReportingCommand<ErrorOr<Shift>>;