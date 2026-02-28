
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate.Events;

public record WashingMachineSpeedStartedEvent(
    Guid Id, string Code, int Speed, SpeedLevel SpeedLevel, DateTimeOffset DateStarted) : IMartenEvent;