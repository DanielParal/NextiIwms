using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate.Events;

public record WashingMachineSpeedCreatedEvent(Guid Id, string Code, int Speed, SpeedLevel SpeedLevel, DateTimeOffset UpdatedAt) : IMartenEvent;