using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate.Events;

public record WashingMachineSpeedEndedEvent(
    Guid Id, DateTimeOffset DateEnded) : IMartenEvent;