using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.CreateShift;

internal record CreateShiftCommand(string NextShiftName, ShiftSchedule NextShiftSchedule) : IReportingCommand<ErrorOr<Shift>>;