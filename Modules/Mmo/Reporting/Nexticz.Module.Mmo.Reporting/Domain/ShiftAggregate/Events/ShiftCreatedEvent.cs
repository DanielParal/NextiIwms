using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;

public record ShiftCreatedEvent(
    Guid Id,
    string Name,
    ShiftSchedule Schedule,
    DateTimeOffset CreatedAt,
    WashingMachine[] WashingMachines) : IMartenEvent;