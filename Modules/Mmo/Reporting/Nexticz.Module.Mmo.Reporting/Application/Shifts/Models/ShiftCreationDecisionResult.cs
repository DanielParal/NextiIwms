using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Models;

internal record ShiftCreationDecisionResult(bool ShouldCreate, string? NextShiftName, ShiftSchedule? NextShiftSchedule);