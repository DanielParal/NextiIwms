using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;

public record LastShiftMovedToNextToLastEvent(Guid LastShiftId) : IMartenEvent;