using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

public record KitWashCycleFinishedEvent(
    Guid Id,
    Guid? SisterKitId,
    Guid BatchId,
    DateTimeOffset StartDate,
    DateTimeOffset FinishedDate,
    TimeSpan OptimalDuration,
    string WorkerName,
    int OrderId,
    int GlobalKitsCount,
    double Efficiency,
    int WashingMachineSpeed,
    SpeedLevel WashingMachineSpeedLevel) : IMartenEvent;