using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.UnapproveShift;

internal record UnapproveShiftCommand(Shift Shift) : IReportingCommand<ErrorOr<Success>>;