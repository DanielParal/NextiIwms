namespace Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;

public record ShiftStatusChangedEvent(Guid ShiftId, DateTimeOffset UpdatedAt, ShiftStatus FromStatus, ShiftStatus ToStatus);